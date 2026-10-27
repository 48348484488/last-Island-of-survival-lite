using UnityEngine;
using UnityEngine.SceneManagement;
using cfg;
using gs.battle.monster.scmsg;

public class MonsterMind : MonoBehaviour
{
	private enum MonsterState
	{
		STAND = 0,
		CHECK = 1,
		WALK = 2,
		WARN = 3,
		CHASE = 4,
		RETURN = 5
	}

	public MonsterInfo MyMonsterInfo;

	private static readonly CSyncMonsterPos cSyncMonsterPos = new CSyncMonsterPos();

	private static readonly CSyncMonsterVelocity cSyncMonsterVeloctity = new CSyncMonsterVelocity();

	private static readonly CSyncMonsterAnimator cSyncMonsterAnimator = new CSyncMonsterAnimator();

	private static readonly CSyncMonsterOrientation cSyncMonsterOrientation = new CSyncMonsterOrientation();

	private GameObject playerUnit;

	private Animator m_animator;

	private Rigidbody Rig;

	private Vector3 m_birthPos;

	private float m_freeMoveRange;

	private float alertRadius;

	private float m_defenceRange;

	private float m_followRange;

	private float m_attackRange;

	public float walkSpeed;

	public float runSpeed;

	public float turnSpeed;

	private MonsterState currentState;

	public float[] actionWeight = new float[3] { 3000f, 3000f, 4000f };

	public float actRestTme;

	private float lastActTime;

	private float diatanceToPlayer;

	private float diatanceToInitial;

	private Quaternion targetRotation;

	private bool is_Warned;

	private bool is_Running;

	private MonsterCfg m_Cfg;

	private void Awake()
	{
		m_animator = GetComponent<Animator>();
		Rig = base.gameObject.AddComponent<Rigidbody>();
	}

	public void Init(int Id)
	{
		m_Cfg = MonsterCfg.Get(Id);
		m_attackRange = m_Cfg.attackRange;
		m_freeMoveRange = m_Cfg.freeMoveRange;
		m_defenceRange = m_Cfg.defenceRange;
		m_followRange = m_Cfg.followRange;
		m_birthPos = new Vector3(MyMonsterInfo.pos.x, MyMonsterInfo.pos.y, MyMonsterInfo.pos.z);
	}

	public BasePlayerController FindOneAttackTarget()
	{
		return null;
	}

	private void Start()
	{
		playerUnit = GameObject.FindGameObjectWithTag("Player");
		m_attackRange = Mathf.Min(m_defenceRange, m_attackRange);
		m_freeMoveRange = Mathf.Min(m_followRange, m_freeMoveRange);
		RandomAction();
	}

	private void RandomAction()
	{
		lastActTime = Time.time;
		float num = Random.Range(0f, actionWeight[0] + actionWeight[1] + actionWeight[2]);
		if (num <= actionWeight[0])
		{
			currentState = MonsterState.STAND;
			m_animator.SetTrigger("Stand");
		}
		else if (actionWeight[0] < num && num <= actionWeight[0] + actionWeight[1])
		{
			currentState = MonsterState.CHECK;
			m_animator.SetTrigger("Check");
		}
		if (actionWeight[0] + actionWeight[1] < num && num <= actionWeight[0] + actionWeight[1] + actionWeight[2])
		{
			currentState = MonsterState.WALK;
			targetRotation = Quaternion.Euler(0f, Random.Range(1, 5) * 90, 0f);
			m_animator.SetTrigger("Walk");
		}
	}

	private void Update()
	{
		switch (currentState)
		{
		case MonsterState.STAND:
			if (Time.time - lastActTime > actRestTme)
			{
				RandomAction();
			}
			EnemyDistanceCheck();
			break;
		case MonsterState.CHECK:
			if (Time.time - lastActTime > m_animator.GetCurrentAnimatorStateInfo(0).length)
			{
				RandomAction();
			}
			EnemyDistanceCheck();
			break;
		case MonsterState.WALK:
			base.transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, targetRotation, turnSpeed);
			if (Time.time - lastActTime > actRestTme)
			{
				RandomAction();
			}
			WanderRadiusCheck();
			break;
		case MonsterState.WARN:
			if (!is_Warned)
			{
				m_animator.SetTrigger("Warn");
				base.gameObject.GetComponent<AudioSource>().Play();
				is_Warned = true;
			}
			targetRotation = Quaternion.LookRotation(playerUnit.transform.position - base.transform.position, Vector3.up);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, targetRotation, turnSpeed);
			WarningCheck();
			break;
		case MonsterState.CHASE:
			if (!is_Running)
			{
				m_animator.SetTrigger("Run");
				is_Running = true;
			}
			base.transform.Translate(Vector3.forward * Time.deltaTime * runSpeed);
			targetRotation = Quaternion.LookRotation(playerUnit.transform.position - base.transform.position, Vector3.up);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, targetRotation, turnSpeed);
			ChaseRadiusCheck();
			break;
		case MonsterState.RETURN:
			targetRotation = Quaternion.LookRotation(m_birthPos - base.transform.position, Vector3.up);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, targetRotation, turnSpeed);
			base.transform.Translate(Vector3.forward * Time.deltaTime * runSpeed);
			ReturnCheck();
			break;
		}
	}

	private void EnemyDistanceCheck()
	{
		diatanceToPlayer = Vector3.Distance(playerUnit.transform.position, base.transform.position);
		if (diatanceToPlayer < m_attackRange)
		{
			SceneManager.LoadScene("Battle");
		}
		else if (diatanceToPlayer < m_defenceRange)
		{
			currentState = MonsterState.CHASE;
		}
		else if (diatanceToPlayer < alertRadius)
		{
			currentState = MonsterState.WARN;
		}
	}

	private void WarningCheck()
	{
		diatanceToPlayer = Vector3.Distance(playerUnit.transform.position, base.transform.position);
		if (diatanceToPlayer < m_defenceRange)
		{
			is_Warned = false;
			currentState = MonsterState.CHASE;
		}
		if (diatanceToPlayer > alertRadius)
		{
			is_Warned = false;
			RandomAction();
		}
	}

	private void WanderRadiusCheck()
	{
		diatanceToPlayer = Vector3.Distance(playerUnit.transform.position, base.transform.position);
		diatanceToInitial = Vector3.Distance(base.transform.position, m_birthPos);
		if (diatanceToPlayer < m_attackRange)
		{
			SceneManager.LoadScene("Battle");
		}
		else if (diatanceToPlayer < m_defenceRange)
		{
			currentState = MonsterState.CHASE;
		}
		else if (diatanceToPlayer < alertRadius)
		{
			currentState = MonsterState.WARN;
		}
		if (diatanceToInitial > m_freeMoveRange)
		{
			targetRotation = Quaternion.LookRotation(m_birthPos - base.transform.position, Vector3.up);
		}
	}

	private void ChaseRadiusCheck()
	{
		diatanceToPlayer = Vector3.Distance(playerUnit.transform.position, base.transform.position);
		diatanceToInitial = Vector3.Distance(base.transform.position, m_birthPos);
		if (diatanceToPlayer < m_attackRange)
		{
			SceneManager.LoadScene("Battle");
		}
		if (diatanceToInitial > m_followRange || diatanceToPlayer > alertRadius)
		{
			currentState = MonsterState.RETURN;
		}
	}

	private void ReturnCheck()
	{
		diatanceToInitial = Vector3.Distance(base.transform.position, m_birthPos);
		if (diatanceToInitial < 0.5f)
		{
			is_Running = false;
			RandomAction();
		}
	}
}
