using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadSafeQueue<T>
{
	private object queueLock;
	private Queue<T> queue;
	
	public ThreadSafeQueue()
	{
		queueLock = new object();
		queue = new Queue<T>();
	}
	
	public void Enqueue(T obj)
	{
		lock(queueLock)
		{
			queue.Enqueue(obj);
		}
	}
	
	public T Peek()
	{
		T obj;
		lock(queueLock)
		{
			obj = queue.Peek();
		}
		return obj;
	}
	
	public T Dequeue()
	{
		T obj;
		lock(queueLock)
		{
			obj = queue.Dequeue();
		}
		return obj;
	}
	
	public int Count {get{int i = 0; lock(queueLock){i = queue.Count;} return i;}}
}
