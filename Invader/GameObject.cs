using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Invader
{
    interface Imovable
    {
        void move();
        void setNextPos(Vector2 next);
    }

    interface IhaveRange
    {
        Area hitBox { get; set; }
    }

    interface IbeDamaged
    {
        void beDamaged();
    }

    class Existence
    {
        private static Existence singleton = new Existence();
        private static List<GameObject> gmObjList;
        public int count { get { return gmObjList.Count; } }
        private Existence()
        {
            gmObjList = new List<GameObject>();
        }

        public static Existence getInstance()
        {
            return singleton;
        }

        public void add(GameObject item)
        {
            gmObjList.Add(item);
        }

        public void remove(GameObject item)
        {
            gmObjList.Remove(item);
        }

        public void moveAll()
        {
            GameObject[] gmObjArr = copyToArray();
            foreach (GameObject item in gmObjArr)
            {
                if (item is MovableObject)
                {
                    MovableObject mvObj = item as MovableObject;
                    mvObj.move();
                }
            }
        }


        //public Task moveAll()
        //{
        //    List<Task> tasks = new List<Task>();
        //    foreach (GameObject item in gmObjList)
        //    {
        //        if (item is MovableObject)
        //        {
        //            MovableObject mvObj = item as MovableObject;
        //            Task task = Task.Run(() => { mvObj.move(); });
        //            tasks.Add(task);
        //        }
        //    }
        //    return Task.WhenAll(tasks);
        //}

        public IEnumerable<GameObject> iterate()
        {
            GameObject[] gmObjArr = copyToArray();
            foreach (GameObject item in gmObjArr)
            {
                yield return item;
            }
        }

        private GameObject[] copyToArray()
        {
            GameObject[] gmObjArr = new GameObject[gmObjList.Count];
            gmObjList.CopyTo(gmObjArr);
            return gmObjArr;
        }

    }

    class DyingExistence
    {
        private static DyingExistence singleton = new DyingExistence();
        private static List<GameObject> gmObjList;
        public int count { get { return gmObjList.Count; } }
        private DyingExistence()
        {
            gmObjList = new List<GameObject>();
        }

        public static DyingExistence getInstance()
        {
            return singleton;
        }

        public void add(GameObject item)
        {
            gmObjList.Add(item);
        }

        public void remove(GameObject item)
        {
            gmObjList.Remove(item);
        }
        public IEnumerable<GameObject> iterate()
        {
            GameObject[] gmObjArr = copyToArray();
            foreach (GameObject item in gmObjArr)
            {
                yield return item;
            }
        }

        private GameObject[] copyToArray()
        {
            GameObject[] gmObjArr = new GameObject[gmObjList.Count];
            gmObjList.CopyTo(gmObjArr);
            return gmObjArr;
        }
    }


    abstract class GameObject : IhaveRange, IbeDamaged
    {
        public Vector2 position { protected set;  get; }
        protected Existence exist = Existence.getInstance();
        protected DyingExistence dying = DyingExistence.getInstance();
        public int teamID { get; protected set; }
        public Area hitBox { get; set; }
        public abstract void beDamaged();
    }

    abstract class MovableObject : GameObject, Imovable
    {
        protected Vector2 nextPosition;
        public virtual void move()
        {
            Vector2 diff = nextPosition - position;
            hitBox.start += diff;
            hitBox.end += diff;
            position = nextPosition;
        }

        public virtual void setNextPos(Vector2 next)
        {
            nextPosition = next;
        }

        public void setInitialPosition(Vector2 pos)
        {
            Vector2 diff = pos - position;
            hitBox.start += diff;
            hitBox.end += diff;
            position = pos;
            nextPosition = pos;
            
        }
    }
}
