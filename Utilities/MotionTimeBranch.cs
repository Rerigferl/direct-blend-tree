﻿namespace Numeira;

public sealed class MotionTimeBranch : MotionBranch
{
    public string BlendParameter { get; set; } = "";

    public MotionTimeBranch(AnimationClip motion) : base(motion) { }

    protected override void Build(BlendTree blendTree, float? threshold)
    {
        if (Motion == null || Motion is not AnimationClip clip)
            return;

            var tree = new BlendTree();
            tree.blendParameter = BlendParameter;
            tree.name = Name ?? clip.name;
            tree.useAutomaticThresholds = false;

        foreach(var x in Enumerate())
            {
                x.Motion.name = $"{clip.name}({x.Threshold:f2})";
                tree.AddChild(x.Motion, x.Threshold);
            }

        blendTree.AddChild(tree, threshold ?? 0);
    }

    private IEnumerable<(AnimationClip Motion, float Threshold)> Enumerate()
    {
        var source = Motion as AnimationClip;
        if (source == null)
            yield break;

        var bindings = AnimationUtility.GetCurveBindings(source);
        var times = new HashSet<float>();
        foreach(var binding in bindings)
        {
            foreach(var key in AnimationUtility.GetEditorCurve(source, binding).keys)
            {
                times.Add(key.time);
            }
        }
        var sortedTimes = times.OrderBy(x => x).ToArray();
        for (int i = 0; i < sortedTimes.Length; i++)
        {
            AnimationClip newClip = new();
            foreach (var binding in bindings)
            {
                var curve = AnimationUtility.GetEditorCurve(source, binding);
                AnimationUtility.SetEditorCurve(newClip, binding, AnimationCurve.Constant(0, 0, curve.Evaluate(sortedTimes[i])));
            }

            yield return (newClip, sortedTimes[i] / source.length);
        }
    }
}