using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator {
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();
    
    public static void Register<T>(T service) {
        if (services.ContainsKey(typeof(T)))
            throw new Exception("Service of type " + typeof(T).ToString() + " is already register in Service Locator");

        services.Add(typeof(T), service);
    }

    public static T Get<T>() {
        if (!services.ContainsKey(typeof(T)))
            throw new Exception("Service Locator doesn't contain service of type: " + typeof(T).ToString());

        return (T) services[typeof(T)];
    }
}
