using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� ��ũ : https://glikmakesworld.tistory.com/2
//���׸� ����
public class G_Singleton<T> : MonoBehaviour where T : G_Singleton<T> //Scene �� �Ѿ���� ������� �ʴ� �̱���
{
	private static T m_Instance = null;
	private static object _syncobj = new object();
	private static bool appIsClosing = false;

	public static T Instance
	{
		get
		{
			if (appIsClosing)
				return null;

			lock (_syncobj)
			{
				if (m_Instance == null)
				{
					T[] objs = FindObjectsOfType<T>();

					if (objs.Length > 0)
						m_Instance = objs[0];

					if (objs.Length > 1)
						Debug.Log("There is more than one " + typeof(T).Name + " in the scene.");

					if (m_Instance == null)
					{
						//������ ù��°�� ������...
						string goName = typeof(T).ToString();
						GameObject a_go = GameObject.Find(goName);
						if (a_go == null)
							a_go = new GameObject(goName);
						m_Instance = a_go.AddComponent<T>();  //Awake()�� ���ʿ��� �߻��ȴ�.
					}
					else
					{
						m_Instance.Init();
					}
				}

				return m_Instance;
			}//lock (_syncobj)
		} // get
	}//public static T Instance

	public virtual void Awake()
	{
		//������ �ι�°�� ���´�.
		Init();
	}

	protected virtual void Init()
	{
		if (m_Instance == null)
		{
			m_Instance = this as T;
			DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			if (m_Instance != this)
			{
				DestroyImmediate(base.gameObject);
			}
		}
	}

	private void OnApplicationQuit()  //�� ���� ����� �߻��Ǵ� �Լ�
	{
		m_Instance = null;
		appIsClosing = true;
	}
}

public class A_Singleton<T> : MonoBehaviour where T : A_Singleton<T>  //Scene �� �Ѿ �� ������� �̱���
{
	private static T m_Instance = null;
	private static object _syncobj = new object();
	private static bool appIsClosing = false;

	public static T Instance
	{
		get
		{
			if (appIsClosing)
				return null;

			lock (_syncobj)
			{
				if (m_Instance == null)
				{
					T[] objs = FindObjectsOfType<T>();

					if (objs.Length > 0)
						m_Instance = objs[0];

					if (objs.Length > 1)
						Debug.Log("There is more than one " + typeof(T).Name + " in the scene.");

					if (m_Instance == null)
					{
						//������ ù��°�� ������...
						string goName = typeof(T).ToString();
						GameObject a_go = GameObject.Find(goName);
						if (a_go == null)
							a_go = new GameObject(goName);
						m_Instance = a_go.AddComponent<T>();  //Awake()�� ���ʿ��� �߻��ȴ�.
					}
					else
					{
						m_Instance.Init();
					}
				}

				return m_Instance;
			}
		}
	}

	public virtual void Awake()
	{
		//������ �ι�°�� ���´�.
		Init();
	}

	protected virtual void Init()
	{
		if (m_Instance == null)
		{
			m_Instance = this as T;
		}
		else
		{
			if (m_Instance != this)
			{
				DestroyImmediate(base.gameObject);
			}
		}
	}

	private void OnApplicationQuit()
	{
		m_Instance = null;
		appIsClosing = true;
	}
}