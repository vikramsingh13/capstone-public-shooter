using Zenject;

public class ProjectBindings: MonoInstaller
{
    //public ProjectileData projectileData;
    //public ItemData itemData;

    public override void InstallBindings()
    {
        //scriptable objects can be bound as such:
        //Container.BindInstance(projectileData).AsSingle().NonLazy();
        //Container.BindInstance(itemData).AsSingle().NonLazy();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerMovement>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<CombatManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<ProjectileManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}



