# Code Conventions

## Naming Conventions

### Class & Variable Naming Rules
- **FORBIDDEN**: Using words `Controller` or `Manager` in class/variable names
- **RECOMMENDED**: Use `Handler`, `Service`, `System`, `Provider`, `Processor` instead
  ```csharp
  // ❌ Bad
  public class WeaponController { }
  private IUnitManager _unitManager;
  
  // ✅ Good  
  public class WeaponHandler { }
  private IUnitService _unitService;
  ```

### Variables
- **Private fields**: `_camelCase` with underscore prefix
  ```csharp
  private float _attackSpeed;
  private readonly List<Unit> _units = new();
  ```

- **Public properties**: `PascalCase`
  ```csharp
  public float AttackSpeed { get; private set; }
  public bool IsAlive => Health > 0;
  ```

- **Constants**: `PascalCase`
  ```csharp
  private const float DefaultAttackInterval = 0.5f;
  private const int MaxUnitsCount = 100;
  private const string SaveFileName = "GameData.json";
  ```

- **SerializeField**: Private with underscore
  ```csharp
  [SerializeField] private GameObject _prefab;
  [field: SerializeField] public float Speed { get; private set; }
  ```

## Magic Numbers & Strings Rule

❌ **FORBIDDEN**: Magic numbers (except 0 and 1) and hardcoded strings
```csharp
// ❌ Bad
if (health < 50) return;
transform.localScale = Vector3.one * 1.5f;
PlayerPrefs.SetString("Language", "en");
```

✅ **CORRECT**: Extract to constants
```csharp
// ✅ Good
private const float CriticalHealthThreshold = 50f;
private const float ScaleMultiplier = 1.5f;
private const string LanguageKey = "Language";
private const string EnglishCode = "en";

if (health < CriticalHealthThreshold) return;
transform.localScale = Vector3.one * ScaleMultiplier;
PlayerPrefs.SetString(LanguageKey, EnglishCode);
```

## Class Structure Order

1. **Constants** (private const)
2. **Static readonly** (hash codes, etc.)
3. **SerializeField variables** (grouped together)
4. **Private readonly fields**
5. **Private fields**
6. **Public properties**
7. **Events**
8. **Constructor/Dependency Injection** ([Inject] methods)
9. **Public methods**
10. **Unity lifecycle** (Awake, Start, Update, OnDestroy)
11. **Private methods**
12. **Nested classes** (Factory classes at bottom)

```csharp
public class ExampleClass : MonoBehaviour
{
    private const float AttackInterval = 1f;
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    
    [SerializeField] private Transform _target;
    [field: SerializeField] public float Speed { get; private set; }
    
    private readonly List<Unit> _units = new();
    private IWeaponSystem _weaponSystem;
    private float _health;
    
    public bool IsAlive => _health > 0;
    public event Action OnDeath;
    
    [Inject]
    private void Construct(IWeaponSystem weaponSystem)
    {
        _weaponSystem = weaponSystem;
    }
    
    public void TakeDamage(float damage) { }
    
    private void Start() { }
    private void Update() { }
    
    private void Die() { }
    
    public class Factory : PlaceholderFactory<ExampleClass> { }
}
```

## Dependency Injection (Zenject)

### Injection Rules
- **ALWAYS use Construct method** for dependency injection in MonoBehaviours
- **Field injection** ([Inject] on fields) - use rarely, only when necessary
- **NEVER use property injection** - forbidden
- **NEVER use Optional injection** - handle null checks manually if needed

### Constructor Injection
```csharp
public class WeaponSystem
{
    private readonly IObjectPool _objectPool;
    private readonly IGameSettings _gameSettings;
    
    public WeaponSystem(IObjectPool objectPool, IGameSettings gameSettings)
    {
        _objectPool = objectPool;
        _gameSettings = gameSettings;
    }
}
```

### Method Injection (Preferred for MonoBehaviours)
```csharp
[Inject]
private void Construct(IResourceProvider resourceProvider, DiContainer container)
{
    _resourceProvider = resourceProvider;
    _container = container;
}
```

### Field Injection (Use Rarely)
```csharp
// Use only when Construct method is not suitable
[Inject] private IService _service;
```

### Factory Pattern
```csharp
public class Factory : PlaceholderFactory<Unit, UnitCharacteristics, AiPathMoving> { }
```

## Formatting Rules

### Brackets
- **New line** for classes, methods, namespaces
- **Same line + braces** for control structures
```csharp
public void Attack()
{
    if (target != null)
    {
        Fire();
    }
    
    for (var i = 0; i < weapons.Count; i++)
    {
        weapons[i].Reload();
    }
}
```

### var Usage
- **Use var** when type is obvious
```csharp
var damage = CalculateDamage();
var units = Physics.OverlapSphere(position, radius);
var config = Resources.Load<WeaponConfig>("Config");
```

- **Use explicit type** when unclear or for interfaces
```csharp
IWeapon weapon = GetWeapon();
Vector3 direction;
float result = ComplexCalculation();
```

### Null Checking
```csharp
// ✅ Preferred
if (target)
if (_units?.Count > 0)

// ❌ Avoid
if (target != null)
if (_units != null && _units.Count > 0)
```

### Collections
```csharp
private readonly List<Unit> _units = new();
public IReadOnlyList<Unit> Units => _units;
```

## SOLID Principles

### Single Responsibility
- One class = one purpose
- One method = one action
```csharp
// ✅ Good - focused responsibility
public class HealthSystem
{
    public void TakeDamage(float damage) { }
    public void Heal(float amount) { }
}

public class MovementSystem
{
    public void Move(Vector3 direction) { }
    public void Stop() { }
}
```

### Dependency Inversion
- Depend on abstractions (interfaces)
- Inject dependencies, don't create them
```csharp
// ✅ Good
public class WeaponController
{
    private readonly ITargetingSystem _targeting; // Abstract dependency
    
    public WeaponController(ITargetingSystem targeting)
    {
        _targeting = targeting;
    }
}

// ❌ Bad
public class WeaponController
{
    private readonly TargetingSystem _targeting = new(); // Concrete dependency
}
```

### Interface Segregation
- Small, focused interfaces
```csharp
public interface IAttackable
{
    void TakeDamage(float damage);
}

public interface IMovable  
{
    void Move(Vector3 direction);
}

public interface ITargetable
{
    Vector3 Position { get; }
    bool IsAlive { get; }
}
```

## Unity Specific

### SerializeField Pattern
```csharp
[field: SerializeField] public float Speed { get; private set; }
[SerializeField] private Transform _target;
```

### Async Operations
Use UniTask for Unity async operations
```csharp
private async UniTask LoadDataAsync(CancellationToken token)
{
    await UniTask.Delay(TimeSpan.FromSeconds(LoadDelay), cancellationToken: token);
}
```

### Hash Codes for Animator
```csharp
private static readonly int AttackTrigger = Animator.StringToHash("Attack");
private static readonly int SpeedParameter = Animator.StringToHash("Speed");
```

## Forbidden Patterns

❌ **NO static classes** (except extensions)  
❌ **NO singletons**  
❌ **NO public fields** (use properties)  
❌ **NO comments explaining obvious code**  
❌ **NO GameObject.Find** in runtime code  
❌ **NO magic numbers** (except 0 and 1)  
❌ **NO hardcoded strings**

## Extension Methods

Static extensions are allowed and encouraged:
```csharp
public static class VectorExtensions
{
    public static Vector3 WithY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }
}

public static class TransformExtensions  
{
    public static void ResetTransform(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
}
```