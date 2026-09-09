using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Service : MonoBehaviour
{
    protected virtual void Awake()
    {
        Type type = this.GetType();
        if (ServiceLocator.TryGetService(type, out Service service) && service != this)
        {
            Destroy(this);
            Debug.LogException(new InvalidOperationException("Cannot instantiate services outside of " + nameof(ServiceLocator)));
        }
    }

    // Fires after all Services have been instantiated, to be used when code relies on other Services
    public virtual void Setup()
    {

    }
}

public static class ServiceLocator
{
    private static Dictionary<Type, Component> _services = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Setup()
    {
        _services.Clear();

        Type[] serviceTypes = typeof(ServiceLocator).Assembly
            .GetTypes()
            .Where(type => typeof(Service).IsAssignableFrom(type) && type != typeof(Service))
            .ToArray();

        foreach (Type service in serviceTypes)
        {
            if (service.IsGenericType || service.IsAbstract) continue; // Only consider actual implementations, not abstract classes
            if (!service.IsSealed) throw new InvalidOperationException("Service has to be a sealed class");

            GameObject serviceObject = new GameObject(service.Name);
            GameObject.DontDestroyOnLoad(serviceObject);
            Component serviceComponent = serviceObject.AddComponent(service);
            _services[service] = serviceComponent;
        }

        foreach (Component service in _services.Values)
        {
            MethodInfo setup = service.GetType().GetMethod(nameof(Service.Setup), BindingFlags.Public | BindingFlags.Instance);
            setup?.Invoke(service, null);
        }
    }

    public static Service GetService(Type serviceType)
    {
        if (_services.TryGetValue(serviceType, out Component serviceComponent)) return serviceComponent as Service;
        else throw new InvalidOperationException("Service not recognized");
    }

    public static TService GetService<TService>() where TService : Service
    {
        if (_services.TryGetValue(typeof(TService), out Component serviceComponent)) return serviceComponent as TService;
        else throw new InvalidOperationException("Service not recognized");
    }

    public static bool TryGetService<TService>(out TService service) where TService : Service
    {
        if (_services.TryGetValue(typeof(TService), out Component serviceComponent))
        {
            service = serviceComponent as TService;
            return true;
        }
        else
        {
            service = null;
            return false;
        }
    }

    public static bool TryGetService(Type serviceType, out Service service)
    {
        if (_services.TryGetValue(serviceType, out Component serviceComponent))
        {
            service = serviceComponent as Service;
            return true;
        }
        else
        {
            service = null;
            return false;
        }
    }
}
