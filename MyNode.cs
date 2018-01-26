using System;

namespace Demo
{
    [Serializable]
    public class MyNode
    {
        public string NodeText;
        public string NodeId;
        public string ParentId;
    }
}
//多個客端可共同編輯同一樹狀圖, 修改後的結果要即時通知所有編輯中的客端

//新建圖

//新增節點, 節點可儲存資料(string)

//編輯節點資料

//刪除節點

//搬移節點