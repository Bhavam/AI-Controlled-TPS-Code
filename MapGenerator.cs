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
  [Range(0,1)]public float obstaclePercent;
  [Range(0,1)] public float outilnePercent; 
  public int seed=10;
  Coord mapCentre;
 /*   public override bool Equals(object obj)
 {
     if (obj is Coord coord)
     {
         return this == coord;
     }

     return false;
 }

 public override int GetHashCode() => new { x, y }.GetHashCode(); */
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
      mapCentre=new Coord((int)mapSize.x/2,(int)mapSize.y/2);
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
     int currentObstacleCount=0;
     bool[,] obstacleMap=new bool[(int)mapSize.x,(int)mapSize.y]; //to keep track of obsctacles in the map and to hel the MapIsFullyAccessible method
     int obstacleCount=(int)(mapSize.x*mapSize.y*obstaclePercent);
     for (int i=0;i<obstacleCount;i++)
       {
         Coord randomCoord=GetRandomCoord();
         obstacleMap[randomCoord.x,randomCoord.y]=true;
         currentObstacleCount++;
         if(randomCoord != mapCentre && MapIsFullyAccessible(obstacleMap,currentObstacleCount))
         {
         Vector3 obstaclePosition=CoordtoPosition(randomCoord.x,randomCoord.y);
         Transform newObstacle=Instantiate(obstaclePrefab,obstaclePosition+Vector3.up*0.5f,Quaternion.identity) as Transform;
         newObstacle.parent=mapHolder; //check to make sure its generating inside mapholder and thus getting destroyed
         }else
         {
             obstacleMap[randomCoord.x,randomCoord.y]=true;
             currentObstacleCount--;
         }

       }  
  }
/*   bool MapIsFullyAccessible(bool[,] obstacleMap,int currentObstacleCount)
  {
        bool[,] mapFlags=new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
        Queue<Coord> queue=new Queue<Coord> (); // a new expression requires () 
        queue.Enqueue(mapCentre);
        mapFlags[mapCentre.x,mapCentre.y]=true;
        int accessibleTileCount=1;
        while(queue.Count !=0)
        {
            Coord tile = queue.Dequeue();
            for (int x=-1;x<1;x++)
              {
                for (int y=-1;y<1;y++)
                   {
                     int neighbourX=tile.x+x;
                     int neighbourY=tile.y+y;
                     if(x==0 || y==0)
                     {
                        if(neighbourX >=0 && neighbourY >=0 && neighbourX <= obstacleMap.GetLength(0) && neighbourY <= obstacleMap.GetLength(1))
                        {
                            if(!(mapFlags[neighbourX,neighbourY]) && !(obstacleMap[neighbourX,neighbourY]))
                            {
                                mapFlags[neighbourX,neighbourY]=true;
                                queue.Enqueue(new Coord(neighbourX,neighbourY));
                                accessibleTileCount++;
                            }
                        }
                     }
                   }
             }  

        }
      int targetAccessableTileCount=(int)(mapSize.x*mapSize.y-currentObstacleCount);
      return targetAccessableTileCount==accessibleTileCount;  
    } */
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {
		bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (mapCentre);
		mapFlags [mapCentre.x, mapCentre.y] = true;

		int accessibleTileCount = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();

			for (int x = -1; x <= 1; x ++) {
				for (int y = -1; y <= 1; y ++) {
					int neighbourX = tile.x + x;
					int neighbourY = tile.y + y;
					if (x == 0 || y == 0) {
						if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) {
							if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX,neighbourY]) {
								mapFlags[neighbourX,neighbourY] = true;
								queue.Enqueue(new Coord(neighbourX,neighbourY));
								accessibleTileCount ++;
							}
						}
					}
				}
			}
		}

		int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
		return targetAccessibleTileCount == accessibleTileCount;
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
		public static bool operator ==(Coord c1, Coord c2) {
			return c1.x == c2.x && c1.y == c2.y;
		}

		public static bool operator !=(Coord c1, Coord c2) {
			return !(c1 == c2);
		}      
  }
}
