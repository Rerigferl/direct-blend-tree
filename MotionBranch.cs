
namespace Numeira;

internal class MotionBranch : IBlendTree
{
    public Motion Motion { get; set; }

    string IBlendTree.Name => Motion.name;

    public MotionBranch(Motion motion)
    {
        Motion = motion;
    }

    void IBlendTree.Append(IBlendTree blendTree, float? threshold) { }
    void IBlendTree.Build(BlendTree blendTree, float? threshold) => Build(blendTree, threshold);

    protected virtual void Build(BlendTree blendTree, float? threshold)
    {
        blendTree.AddChild(Motion, threshold ?? 0);
    }
}
