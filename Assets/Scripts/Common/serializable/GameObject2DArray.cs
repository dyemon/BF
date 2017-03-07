 public class GameObject2DArray : MonoBehaviour, ISerializationCallbackReceiver
 {
     public GameObject[,] items;
 
     [HideInInspector]
     [SerializeField]
     private GameObject[] m_FlattendMapLayout;
 
     [HideInInspector]
     [SerializeField]
     private int m_FlattendMapLayoutRows;
  
     public void OnBeforeSerialize() {
         int c1 = items.GetLength(0);
         int c2 = items.GetLength(1);
         int count = c1*c2;
         m_FlattendMapLayout = new GameObject[count];
         m_FlattendMapLayoutRows = c1;
         for(int i = 0; i < count; i++) {
             m_FlattendMapLayout[i] = items[i % c1, i / c1];
         }
     }
	 
     public void OnAfterDeserialize() {
         int count = m_FlattendMapLayout.Length;
         int c1 = m_FlattendMapLayoutRows;
         int c2 = count / c1;
         items = new GameObject[c1,c2];
         for(int i = 0; i < count; i++) {
             items[i % c1, i / c1] = m_FlattendMapLayout[i];
         }
     }
 }