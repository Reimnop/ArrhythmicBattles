using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Game.Content.Props;
using ArrhythmicBattles.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content;

public delegate Prop PropFactory(ContentLoader contentLoader, PhysicsWorld physicsWorld, Vector3 position, Vector3 scale, Quaternion rotation);

public class PropContent
{
    public Registry<PropFactory> Registry { get; }

    public PropContent()
    {
        // Register all prop factories here
        var builder = new RegistryBuilder<PropFactory>()
            .Add(new Identifier("grid"), () => (contentLoader, physicsWorld, position, scale, rotation) => new GridProp(contentLoader, physicsWorld, position, scale, rotation));

        Registry = builder.Build();
    }
}