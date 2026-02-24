using UnityEngine;
using UnityEngine.Tilemaps;

// Bu satır sayesinde Project penceresinde sağ tıkla oluşturabileceksin
[CreateAssetMenu(fileName = "New Desert Theme", menuName = "Themes/Desert Theme")]
public class DesertThemeFactory : ThemeFactory
{
    [Header("Çöl Teması Tuğlaları")]
    public TileBase sandGround;       // Çöl kumu
    public TileBase stoneBlock;       // Kırılmaz taş
    public TileBase cactusBox;        // Kırılabilir kutu/kaktüs
    public TileBase ancientWall;      // Sert duvar

    [Header("Düşman")]
    public GameObject desertEnemyPrefab;

    public override TileBase GetGround() => sandGround;
    public override TileBase GetUnbreakableWall() => stoneBlock;
    public override TileBase GetBreakableWall() => cactusBox;
    public override TileBase GetHardWall() => ancientWall;

    public override GameObject GetEnemyPrefab() => desertEnemyPrefab;
}