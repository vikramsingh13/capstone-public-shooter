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
    }
}



