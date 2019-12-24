using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
  List<Coord> allTileCoords;
  Queue<Coord> shuffledTileCoords;
  public Transform tilePrefab;
  public Transform obstaclePrefab;
  public Vector2 mapSize;
  [Range(0,1)] public float outilnePercent; 
  public int seed=10;
  void Start()
  {
      GenerateMap();
  }
  public void GenerateMap()
  {
      allTileCoords=new List<Coord>();
      for(int x=0;x<mapSize.x;x++)
      {
          for(int y=0;y<mapSize.y;y++)
          {
              allTileCoords.Add(new Coord(x,y));
          }
      }      
      shuffledTileCoords=new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(),seed));
      string holderName="Generated Map";//used to assign name to the mapHolder object 
      if(transform.FindChild(holderName)) // continuously checks for an object named Gnerated Map 
      {
       DestroyImmediate(transform.FindChild(holderName).gameObject); // DestroyImmediate is used to to effect this every call during the game code is being executed
      }

      Transform mapHolder=new GameObject(holderName).transform; //creates a game object under which the map is stored 
      mapHolder.parent=transform; // this fucks up the generation of new maps hierarchial spot pls mind in future makes new map inside mapHolder
      for(int x=0;x<mapSize.x;x++)
      {
          for(int y=0;y<mapSize.y;y++)
          {
              Vector3 tilePosition=CoordtoPosition(x,y); //why does -mapSize.x/2 work
              Transform newTile=Instantiate(tilePrefab,tilePosition,Quaternion.Euler(Vector3.right*90)) as Transform;//override object type as Transform
              newTile.localScale=Vector3.one*(1-outilnePercent);    //localScale works by assigning scale to each individual tiles giving effect of outline     
              newTile.parent=mapHolder; // each new mapTile is made a child of the map holder object 
          }
      }
     int obstacleCount=10;
     for (int i=0;i<obstacleCount;i++)
       {
         Coord randomCoord=GetRandomCoord();
         Vector3 obstaclePosition=CoordtoPosition(randomCoord.x,randomCoord.y);
         Transform newObstacle=Instantiate(obstaclePrefab,obstaclePosition+Vector3.up*0.5f,Quaternion.identity) as Transform;
         newObstacle.parent=mapHolder; //check to make sure its generating inside mapholder and thus getting destroyed
       }  
  }
  Vector3 CoordtoPosition(int x,int y)
  {
     return new Vector3(-mapSize.x/2+0.5f+x,0,-mapSize.y/2+0.5f+y);
  }
  public Coord GetRandomCoord()
  {
      Coord randomCoord=shuffledTileCoords.Dequeue();
      shuffledTileCoords.Enqueue(randomCoord);
      return randomCoord;
  }
  public struct Coord{  //structure for coordinates
      public int x,y;
      public Coord(int _x,int _y) //supposed to be a constructor check how does it work
      {
          x=_x;
          y=_y;
      }
  }
}
