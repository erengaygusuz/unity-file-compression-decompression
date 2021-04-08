using UnityEngine.Events;

public class GenericEvent : UnityEvent { }
public class GenericEvent<T> : UnityEvent<T>{}
public class GenericEvent<T, U> : UnityEvent<T, U>{}
public class GenericEvent<T, U, V> : UnityEvent<T, U, V>{}
public class GenericEvent<T, U, V, Y> : UnityEvent<T, U, V, Y>{}