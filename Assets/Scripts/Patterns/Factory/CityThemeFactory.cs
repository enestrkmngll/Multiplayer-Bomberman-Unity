using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New City Theme", menuName = "Themes/City Theme")]
public class CityThemeFactory : ThemeFactory
{
    [Header("Þehir Temasý Tuðlalarý")]
    public TileBase concreteGround;
    public TileBase brickWall;        
    public TileBase trashBin;         
    public TileBase metalBlock;       

    [Header("Düþman")]
    public GameObject cityEnemyPrefab;

    public override TileBase GetGround() => concreteGround;
    public override TileBase GetUnbreakableWall() => brickWall;
    public override TileBase GetBreakableWall() => trashBin;
    public override TileBase GetHardWall() => metalBlock;

    public override GameObject GetEnemyPrefab() => cityEnemyPrefab;


}