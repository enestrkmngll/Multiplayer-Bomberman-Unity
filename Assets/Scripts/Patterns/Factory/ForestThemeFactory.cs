using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Forest Theme", menuName = "Themes/Forest Theme")]
public class ForestThemeFactory : ThemeFactory
{
    [Header("Orman Temasý Tuðlalarý")]
    public TileBase grassGround;      
    public TileBase treeWall;        
    public TileBase logBox;          
    public TileBase hardRock;         
    [Header("Düþman")]
    public GameObject forestEnemyPrefab;

    public override TileBase GetGround() => grassGround;
    public override TileBase GetUnbreakableWall() => treeWall;
    public override TileBase GetBreakableWall() => logBox;
    public override TileBase GetHardWall() => hardRock;

    public override GameObject GetEnemyPrefab() => forestEnemyPrefab;
}