using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class ThemeFactory : ScriptableObject
{
    public abstract TileBase GetGround();        
    public abstract TileBase GetUnbreakableWall(); 
    public abstract TileBase GetBreakableWall();   
    public abstract TileBase GetHardWall();       

    public abstract GameObject GetEnemyPrefab();
}