# Object Pooling 
***
The problem in this exercise was that GameObjects are Instantiated frequently. These are
heavy instructions that can be avoided. To avoid this, we can use a method or a pattern
called Object Pooling. Object pooling means instead of deleting and creating new bullets, we
simply set them to true and then to false accordingly. Let’s compare it to a bookshelf. We
have a bookshelf that has a certain capacity for how many books can be stored. So when we
take a book from it to read it, we put it back. That’s kind of the same with object pooling.
Instead of taking the book off the shelf, reading it, and destroying it, just to go to the store to
buy the same book again to put it back on the shelf. We don’t want to do that. It would be
heavy on our economy to do that. Just like creating and destroying GameObjects every time
they are used would be performance-heavy. 
***
### The object pooling pattern draws from a collection of deactivated objects instead of instantiating new ones
***
Unity has its own API for Object Pooling called Pool. 

` using UnityEngine.Pool; `

![ObjectPool](https://github.com/forsbergsskola-se/gp23-206-design-patterns-philipbrankounni/blob/main/Assets/Scripts/P1_Pooling/ObjectPool.png)

In Unity, the hierarchy is filled with a certain amount of gameObjects (in this case it’s Bullet and Enemy) that we can SetActive to TRUE or FALSE whenever we want to instantiate them.

**Object pooling has two parts involved:**

* An ObjectPool that holds the collection of GameObjects to draw from (Gun, EnemySpawner or PlatformSpawner…)
* PooledObject component added to the Prefab. This helps each cloned item to keep its reference to the pool (Bullet, Enemy, Platform…)

To make a pool, first, we need a gameObject that we are going to store in our pool. So we need to create a script for Bullet. (We are going to use the example gun/castle and bullets/projectiles)
After creating the Bullet script, we can go to the bullet holder, in this case, a gun. In a Gun script, we need to write:

`using UnityEngine.Pool;`

`private IObjectPool<Bullet> objectPool;` – Declaration of the pool containing Bullets

`private bool collectionCheck = true;` – Checks if we’re returning the existing item into the pool

`private int defaultCapacity = 20;` – How many Bullets are going to be stored in the pool

`private int maxSize = 100;` – The pool cap, Bullet that is instantiated after 100 gets destroyed

```c++
private void Awake()
{
    objectPool = new ObjectPool<Bullet>(CreateBullet, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
}
```

The Awake function holds the creation of the pool! ***(dun dun dun)***

We are passing a couple of things as arguments in our `new ObjectPool<Bullet>`, a couple of functions and variables. Let’s go through them.

* CreateBullet - A function where we create our gameObjects to populate the pool by
`Bullet bullet = Instantiate(bulletPrefab)` and putting it in the pool
`bullet.ObjectPool = objectPool`
* OnGetFromPool - A function where we use our gameObjects from the pool by
`SetActive = TRUE`
* OnReleaseToPool - A function where we return our gameObjects to the pool by
`SetActive = FALSE`
* OnDestroyPooledObject - A function where we destroy the pooled object if exceeds
the maxSize by `Destroy(pooledObject.gameObject)`
* collectionCheck - A variable that checks if we’re returning existing item into the pool
* defaultCapacity - A variable of how many objects are going to be stored in the pool
* maxSize - A variable that holds the pool cap, objects that are instantiated after this
number will get destroyed after being used

Now that we know what each of these arguments is, it is time to write this code out!!!
```c++
private Bullet CreateProjectile()
{
    Bullet bullet= Instantiate(bulletPrefab);
    bullet.ObjectPool = objectPool;
    return bullet;
}

private void OnReleaseToPool(Bullet pooledObject) //pooledObject is a bullet (rename)
{
    pooledObject.gameObject.SetActive(false);
}

private void OnGetFromPool(Bullet bullet)
{
    bullet.gameObject.SetActive(true);
}

private void OnDestroyPooledObject(Bullet bullet)
{
    Destroy(bullet.gameObject);
}
```

You can use this script to make your own Gun script and have the bullets stored in the pool. 

As you can see, the logic behind Object Pooling is quite simple:
* First, create a pooled item to populate the pool
* Take an item from the pool and use it
* Return an item to the pool
* Destroy a pooled object (just in case it exceeds the maximum capacity)

The projectile script gets a small modification to keep a reference to the ObjectPool. This
makes releasing the object back into the pool a little more convenient.

```c++
public class Bullet: MonoBehaviour
{
…
    private IObjectPool<Bullet> objectPool;
    public IObjectPool ObjectPool { set => objectPool = value; }
…
}
```

Every time you instantiate a large number of objects, you run the risk of causing a small
pause from a garbage-collection spike due to the memory allocation. An object pool solves
this issue to keep your gameplay smooth. You can pre-instantiate your object pool at a good
time, like a loading screen so the player can’t notice the stutter.
***
So let’s sum this up… Object pooling is an optimization technique to relieve the CPU when
creating and destroying a lot of GameObjects. The object pool pattern instantiates
deactivated gameObjects into the pool.

When we need a gameObject, our program doesn’t need to instantiate a new gameObject.
Instead, we request a gameObject from the pool of deactivated ones and enable it.
When done using it, we deactivate the object and return it to the pool instead of destroying it.
***
Just like we saw, Unity has its own API that makes setting up object pools really easy and
fast. There are ways to make this pattern work without the API, but when something is
served, you take it.. right? `:D`
