public class TimedObject<T>
{
	float time = 0f;
	T obj;
	
	public T Value {get{return obj;}}
	public float Time {get{return time;}}
	
	public TimedObject(T o,float t)
	{
		time = t;
		obj = o;
	}
	
	public float Update(float deltaTime)
	{
		time -= deltaTime;
		return time;
	}
}
