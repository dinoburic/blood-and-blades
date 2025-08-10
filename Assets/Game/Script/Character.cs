using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    public float MoveSpeed = 5f;
    private Vector3 _movementVelocity;
    private PlayerInput _playerInput;
    private float _verticalVelocity;
    public float Gravity = -9.8f;
    private Animator _animator;
    public int Coin;


    //Sound
    public AudioClip[] FootstepClips;
    public AudioClip AttackClip;

    private AudioSource _audioSource;

    public AudioClip HitClip;

    public AudioClip EnemyHitClip;

    public AudioClip EnemyDeathClip;
    public AudioClip CoinPickUpClip;
    public AudioClip HealOrbPickUpClip;
    public AudioClip FireballCastClip;
    public AudioClip _enemyHitSound;
    public AudioClip SpawnSound;




    //Enemy
    public bool IsPlayer = true;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform TargetPlayer;

    //Health
    public Health _health;
    private bool isDeadProcessed = false;

    //Damage caster
    private DamageCaster _damageCaster;

    //Player slides
    private float attackStartTime;
    public float AttackSlideDuration = 0.4f;
    public float AttackSlideSpeed = 0.06f;
    private Vector3 impactOnCharacter;
    public bool IsInvincible;
    public float invincibleDuration = 2f;

    private float AttackAnimationDuration;
    public float SlideSpeed = 9f;


    //State Machine
    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit, Slide, Spawn
    }
    public CharacterState CurrentState;
    public float SpawnDuration = 2f;
    private float currentSpawnTime;

    //Material animation
    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    public GameObject ItemToDrop;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();

        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

        _audioSource = GetComponent<AudioSource>();


        if (!IsPlayer)
        {
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = MoveSpeed;
            SwitchStateTo(CharacterState.Spawn);
        }
        else
        {
            _playerInput = GetComponent<PlayerInput>();
           
        }
    }

    private void CalculatePlayerMovement()
    {
        if (_playerInput.MouseButtonDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }
        else if (_playerInput.SpaceKeyDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Slide);
            return;
        }

        _movementVelocity.Set(_playerInput.HorizontalInput, 0f, _playerInput.VerticalInput);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;

        _animator.SetFloat("Speed", _movementVelocity.magnitude);
        _movementVelocity *= MoveSpeed * Time.deltaTime;

        if (_movementVelocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_movementVelocity);

        _animator.SetBool("AirBorne", !_cc.isGrounded);
    }

    private void CalculateEnemyMovement()

    {

        if (TargetPlayer == null || TargetPlayer.gameObject == null)
            return;
        
        float distanceToPlayer = Vector3.Distance(TargetPlayer.position, transform.position);

        if (distanceToPlayer <= 7f) // The unit that stop the enemy to follow player and starts walk randomly

        {

            if (distanceToPlayer >= _navMeshAgent.stoppingDistance)

            {

                _navMeshAgent.destination = TargetPlayer.position;
                _animator.SetFloat("Speed", 0.2f);

            }

            else

            {

                _navMeshAgent.destination = transform.position;
                _animator.SetFloat("Speed", 0f);

                SwitchStateTo(CharacterState.Attacking);
            }

        }

        else

        {

            if (Mathf.Abs(_navMeshAgent.velocity.sqrMagnitude) < float.Epsilon)

            {

                Vector3 randomDirection = Random.insideUnitSphere * 10f;

                UnityEngine.AI.NavMeshHit hit;

                UnityEngine.AI.NavMesh.SamplePosition(transform.position + randomDirection, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas);

                _navMeshAgent.destination = hit.position;

            }

            _animator.SetFloat("Speed", 0.2f);

        }

    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                if (IsPlayer)
                    CalculatePlayerMovement();
                else
                    CalculateEnemyMovement();
                break;
            case CharacterState.Attacking:
                if (IsPlayer)
                {

                    if (Time.time < attackStartTime + AttackSlideDuration)
                    {
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / AttackSlideDuration;
                        _movementVelocity = Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);
                    }

                    if (_playerInput.MouseButtonDown && _cc.isGrounded)
                    {
                        string currentClipName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        AttackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                        if (currentClipName != "LittleAdventurerAndie_ATTACK_03" && AttackAnimationDuration > 0.5f && AttackAnimationDuration < 0.7f)
                        {
                            _playerInput.MouseButtonDown = false;
                            SwitchStateTo(CharacterState.Attacking);

                            //CalculatePlayerMovement();
                        }
                    }
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                _movementVelocity = transform.forward * SlideSpeed * Time.deltaTime;
                break;
            case CharacterState.Spawn:
                currentSpawnTime -= Time.deltaTime;
                if (currentSpawnTime <= 0)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
        }

        if (impactOnCharacter.magnitude > 0.2f)
        {
            _movementVelocity = impactOnCharacter * Time.deltaTime;
        }
        impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);

        if (IsPlayer)
        {
            if (_cc.isGrounded == false)
            {
                _verticalVelocity = Gravity;
            }
            else
            {
                _verticalVelocity = Gravity * 0.3f;
            }

            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;

            _cc.Move(_movementVelocity);
            _movementVelocity = Vector3.zero;
        }
        else
        {
            if (CurrentState != CharacterState.Normal && _cc.enabled)
            {
                _cc.Move(_movementVelocity);
                _movementVelocity = Vector3.zero;
            }
        }
    }

    public void SwitchStateTo(CharacterState newState)
    {
        if (IsPlayer)
        {
            _playerInput.ClearCache();
        }

        //Exiting state
        switch (CurrentState)
        {
            case CharacterState.Normal: break;
            case CharacterState.Attacking:
                if (_damageCaster != null)
                    DisableDamageCaster();

                if (IsPlayer)
                {
                    GetComponent<PlayerVFXManager>().StopBlade();
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Spawn:
                IsInvincible = false;
                break;
        }

        //New state
        switch (newState)
        {
            case CharacterState.Normal: break;
            case CharacterState.Attacking:
                if (!IsPlayer)
                {
                    Quaternion newRotation = Quaternion.LookRotation(TargetPlayer.position - transform.position);
                    transform.rotation = newRotation;
                }
                _animator.SetTrigger("Attack");

                if (IsPlayer)
                {
                    attackStartTime = Time.time;
                    RotateToCursor();

                    if (AttackClip != null)
                      {
                      _audioSource.PlayOneShot(AttackClip);
                     }
                }
                break;
            case CharacterState.Dead:
                if (isDeadProcessed) return;
                isDeadProcessed = true;
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                if (!IsPlayer)
                {
                    SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();
                    mesh.gameObject.layer = 0;
                    
                    if (EnemyDeathClip != null && _audioSource != null)
                    {
                        _audioSource.PlayOneShot(EnemyDeathClip);
                    }
                   
                }

                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");

                if (IsPlayer)
                {
                    IsInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
            case CharacterState.Slide:
                _animator.SetTrigger("Slide");
                break;
            case CharacterState.Spawn:
                IsInvincible = true;
                currentSpawnTime = SpawnDuration;
                StartCoroutine(MaterialAppear());
                if (SpawnSound != null && _audioSource != null)
                {
                    _audioSource.PlayOneShot(SpawnSound);
                }
                break;
        }

        CurrentState = newState;
    }

    public void SlideAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void AttackAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void BeingHitAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void ApplyDamage(int damage, Vector3 attackerPos = new Vector3())
    {
        int Instancedamage;
        if (WeaponManager.Instance.currentDamage==30)
        {
            Instancedamage = 30;
        }
        else
        {
            Instancedamage = WeaponManager.Instance.currentDamage;
        }
        if (IsInvincible)
        {
            return;
        }
        if (_health != null)
        {
            if (IsPlayer)
            {
                _health.ApplyDamage(damage);
            } else
            {
                _health.ApplyDamage(Instancedamage);
            }
        }

        if (!IsPlayer)
        {
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackerPos);
        }

        StartCoroutine(MaterialBlink());

        if (IsPlayer)
        {
            if (HitClip != null)
             {
                _audioSource.PlayOneShot(HitClip);
                }

            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackerPos, 10f);


        }
        else
        {
            if (EnemyHitClip != null)
                {
                    _audioSource.PlayOneShot(EnemyHitClip);
                }

            AddImpact(attackerPos, 2.5f);
        }
    }

    IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(invincibleDuration);
        IsInvincible = false;

        if (CurrentState == CharacterState.BeingHit)
        {
            SwitchStateTo(CharacterState.Normal);
        }
    }

    private void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        impactOnCharacter = impactDir * force;
    }

    public void EnableDamageCaster()
    {
        _damageCaster.EnableDamageCaster();
    }

    public void DisableDamageCaster()
    {
        _damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialBlink()
    {
        _materialPropertyBlock.SetFloat("_blink", 0.4f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);

        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2);

        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0;
        float dissolveHeight_start = 20f;
        float dissolveHeight_target = -10f;
        float dissolveHeight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_start, dissolveHeight_target,
                currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }

        DropItem();
        Destroy(gameObject);
    }

    public void DropItem()
    {
        if (ItemToDrop != null)
        {
            Instantiate(ItemToDrop, transform.position, Quaternion.identity);
        }
    }

    public void PickUpItem(PickUp item)
    {
        switch (item.Type)
        {
            case PickUp.PickUpType.Heal:
                AddHealth(item.Value);
                if (HealOrbPickUpClip != null && _audioSource != null)
                {
                    _audioSource.PlayOneShot(HealOrbPickUpClip);
                }
                break;
            case PickUp.PickUpType.Coin:
                AddCoin(item.Value);
                if (CoinPickUpClip != null && _audioSource != null)
                {
                    _audioSource.PlayOneShot(CoinPickUpClip);
                }
                break;
        }
    }

    private void AddHealth(int health)
    {
        _health.AddHealth(health);

        GetComponent<PlayerVFXManager>().PlayHealVFX();
    }

    private void AddCoin(int coin)
    {
        Coin += coin;
    }

    public void RotateToTarget()
    {
        if (CurrentState != CharacterState.Dead)
        {
            transform.LookAt(TargetPlayer, Vector3.up);
        }
    }

    IEnumerator MaterialAppear()
    {
        float dissolveTimeDuration = SpawnDuration;
        float currentDissolveTime = 0;
        float dissolveHeight_start = -10f;
        float dissolveHeight_target = 20f;
        float dissolveHeight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        
       

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_start, dissolveHeight_target,
                currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
        
      

        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;
        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cursorPos, 1);
        }
    }

    private void RotateToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;
        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            transform.rotation = Quaternion.LookRotation(cursorPos - transform.position, Vector3.up);
        }
    }
    
    public void PlayFootstep()
{
    if (FootstepClips.Length > 0)
    {
        int index = Random.Range(0, FootstepClips.Length);
        _audioSource.PlayOneShot(FootstepClips[index]);
    }
}
    
    public void Die()
    {
        
        Debug.Log($"{name} is dead!");
        SwitchStateTo(CharacterState.Dead);
    }

    public void PlayEnemyHitSound()
    {
        if(_audioSource!=null && _enemyHitSound!=null)
         _audioSource.PlayOneShot(_enemyHitSound);
    }

}
