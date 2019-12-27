using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
  public Map[] maps;
  public int mapIndex;
  List<Coord> allTileCoords;
  Queue<Coord> shuffledTileCoords;
  public Transform tilePrefab;
  public Transform obstaclePrefab;
  public Transform navmeshFloor;
  public Transform navmeshMaskPrefab;
  public Vector2 maxMapSize;
  public float tileSize;
  [Range(0,1)] public float outilnePercent;  // removed currentMap.mapSize,seed,obstaclePercent,mapCentre

  Map currentMap;
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
      currentMap=maps[mapIndex];
      System.Random prng=new System.Random(currentMap.seed);
      allTileCoords=new List<Coord>();
      GetComponent<BoxCollider>().size = new Vector3 (currentMap.mapSize.x * tileSize, .05f, currentMap.mapSize.y * tileSize);
      for(int x=0;x<currentMap.mapSize.x;x++)
      {
          for(int y=0;y<currentMap.mapSize.y;y++)
          {
              allTileCoords.Add(new Coord(x,y));
          }
      }      
      shuffledTileCoords=new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(),currentMap.seed));
      string holderName="Generated Map";//used to assign name to the mapHolder object 
      if(transform.FindChild(holderName)) // continuously checks for an object named Gnerated Map 
      {
       DestroyImmediate(transform.FindChild(holderName).gameObject); // DestroyImmediate is used to to effect this every call during the game code is being executed
      }

      Transform mapHolder=new GameObject(holderName).transform; //creates a game object under which the map is stored 
      mapHolder.parent=transform; // this fucks up the generation of new maps hierarchial spot pls mind in future makes new map inside mapHolder
      for(int x=0;x<currentMap.mapSize.x;x++)
      {
          for(int y=0;y<currentMap.mapSize.y;y++)
          {
              Vector3 tilePosition=CoordtoPosition(x,y); //why does -currentMap.mapSize.x/2 work
              Transform newTile=Instantiate(tilePrefab,tilePosition,Quaternion.Euler(Vector3.right*90)) as Transform;//override object type as Transform
              newTile.localScale=Vector3.one*(1-outilnePercent)*tileSize;    //localScale works by assigning scale to each individual tiles giving effect of outline     
              newTile.parent=mapHolder; // each new mapTile is made a child of the map holder object 
          }
      }
     int currentObstacleCount=0;
     bool[,] obstacleMap=new bool[(int)currentMap.mapSize.x,(int)currentMap.mapSize.y]; //to keep track of obsctacles in the map and to hel the MapIsFullyAccessible method
     int obstacleCount=(int)(currentMap.mapSize.x*currentMap.mapSize.y*currentMap.obstaclePercent);
     for (int i=0;i<obstacleCount;i++)
       {
         Coord randomCoord=GetRandomCoord();
         obstacleMap[randomCoord.x,randomCoord.y]=true;
         currentObstacleCount++;
         if(randomCoord != currentMap.mapCentre && MapIsFullyAccessible(obstacleMap,currentObstacleCount))
         {
         float obstacleHeight=Mathf.Lerp(currentMap.minObstacleHeight,currentMap.maxObstacleHeight,(float)prng.NextDouble());
         Vector3 obstaclePosition=CoordtoPosition(randomCoord.x,randomCoord.y);
         Transform newObstacle=Instantiate(obstaclePrefab,obstaclePosition+Vector3.up*obstacleHeight/2f,Quaternion.identity) as Transform;
         newObstacle.parent=mapHolder; //check to make sure its generating inside mapholder and thus getting destroyed
         newObstacle.localScale=new Vector3((1-outilnePercent)*tileSize,obstacleHeight,(1-outilnePercent)*tileSize); //sizing for the obstacle
         Renderer obstacleRenderer=newObstacle.GetComponent<Renderer>();
         Material obstacleMaterial=new Material(obstacleRenderer.sharedMaterial);
         float colourPercent=(float)randomCoord.y/currentMap.mapSize.y;
         obstacleMaterial.color=Color.Lerp(currentMap.foregroundColour,currentMap.backgroundColour,colourPercent);         
         obstacleRenderer.sharedMaterial=obstacleMaterial;

         }else
         {
             obstacleMap[randomCoord.x,randomCoord.y]=true;
             currentObstacleCount--;
         }

       }  
       Transform maskLeft=Instantiate(navmeshMaskPrefab,Vector3.left*(currentMap.mapSize.x+maxMapSize.x)/4*tileSize,Quaternion.identity) as Transform;
       maskLeft.parent=mapHolder;
       maskLeft.localScale = new Vector3 ((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;
       Transform maskRight = Instantiate (navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4 * tileSize, Quaternion.identity) as Transform;
	  maskRight.parent = mapHolder;
		maskRight.localScale = new Vector3 ((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

		Transform maskTop = Instantiate (navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4 * tileSize, Quaternion.identity) as Transform;
		maskTop.parent = mapHolder;
		maskTop.localScale = new Vector3 (maxMapSize.x, 1, (maxMapSize.y-currentMap.mapSize.y)/2f) * tileSize;

		Transform maskBottom = Instantiate (navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4 * tileSize, Quaternion.identity) as Transform;
		maskBottom.parent = mapHolder;
		maskBottom.localScale = new Vector3 (maxMapSize.x, 1, (maxMapSize.y-currentMap.mapSize.y)/2f) * tileSize;
       navmeshFloor.localScale=new Vector3(maxMapSize.x,maxMapSize.y)*tileSize;
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
      int targetAccessableTileCount=(int)(currentMap.mapSize.x*currentMap.mapSize.y-currentObstacleCount);
      return targetAccessableTileCount==accessibleTileCount;  
    } */
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {
		bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)]; //supplement boolean depiction of map  
		Queue<Coord> queue = new Queue<Coord> (); // a new expression requires ()
		queue.Enqueue (currentMap.mapCentre);
		mapFlags [currentMap.mapCentre.x, currentMap.mapCentre.y] = true;

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

		int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
		return targetAccessibleTileCount == accessibleTileCount;
	}
  Vector3 CoordtoPosition(int x,int y)
  {
     return new Vector3(-currentMap.mapSize.x/2f+0.5f+x,0,-currentMap.mapSize.y/2f+0.5f+y);
  }
  public Coord GetRandomCoord()
  {
      Coord randomCoord=shuffledTileCoords.Dequeue();
      shuffledTileCoords.Enqueue(randomCoord);
      return randomCoord;
  }
 [System.Serializable] public struct Coord{  //structure for coordinates
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
   [System.Serializable] public class Map
    {
             public Coord mapSize;
             [Range(0,1)]
             public float obstaclePercent;
             public int seed;
             public float minObstacleHeight;
             public float maxObstacleHeight;
             public Color foregroundColour;
             public Color backgroundColour;

             public Coord mapCentre
               {            
                  get {
                        return new Coord(mapSize.x/2,mapSize.y/2);
                      }
               }
}
}

