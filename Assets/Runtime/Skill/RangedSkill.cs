using EditorExtend.GridEditor;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 在一定范围内释放的技能，默认作用范围为单点
/// </summary>
[CreateAssetMenu(fileName = "单体技能", menuName = "技能/单体技能", order = -1)]
public class RangedSkill : AimSkill
{
    public int castingDistance = 1;
    public bool aimAtSelf;

    /// <summary>
    /// 获取可选施放位置
    /// </summary>
    public override void GetOptions(PawnEntity agent, IsometricGridManager igm, Vector3Int position, List<Vector3Int> ret)
    {
        base.GetOptions(agent, igm, position, ret);
        List<Vector2Int> primitive = IsometricGridUtility.WithinProjectManhattanDistance(castingDistance);
        if (aimAtSelf)
            primitive.Add(Vector2Int.zero);
        for (int i = 0; i < primitive.Count; i++)
        {
            Vector3Int option = igm.AboveGroundPosition((Vector2Int)position + primitive[i]);
            if (FilterOption(agent, igm, position, option))
                ret.Add(option);
        }
    }

    /// <summary>
    /// 判断某个位置是否是可选的释放范围
    /// </summary>
    public virtual bool FilterOption(PawnEntity agent, IsometricGridManager igm, Vector3Int position, Vector3Int option)
    {
        if (!igm.Contains((Vector2Int)option))    //判断是否在地图内
            return false;
        if (!LayerCheck(position, option))  //判断释放位置高度差是否过大
            return false;
        return true;
    }

    public override void MockArea(IsometricGridManager igm, Vector3Int position, Vector3Int target, List<Vector3Int> ret)
    {
        ret.Clear();
        if (igm.Contains((Vector2Int)target))
            ret.Add(target);
    }

    protected override void Describe(StringBuilder sb)
    {
        base.Describe(sb);
        DescribeCastingDistance(sb);
        DescribeArea(sb);
    }

    protected virtual void DescribeCastingDistance(StringBuilder sb)
    {
        sb.Append("施放范围:");
        sb.Append(castingDistance);
        sb.AppendLine();
        sb.Append("施放高度差:");
        sb.Append(minLayer);
        sb.Append("~");
        sb.Append(maxLayer.ToString("+0"));
        sb.AppendLine();
    }

    protected virtual void DescribeArea(StringBuilder sb)
    {
        sb.Append("单体技能");
        sb.AppendLine();
    }
}
