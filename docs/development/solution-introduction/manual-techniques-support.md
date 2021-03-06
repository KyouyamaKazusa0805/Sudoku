## 基本技巧支持

目前程序支持如下的技巧项：

* 直观技巧（Direct Methods）
  * 排除（Hidden Single）
    * 同数剩余（Last Digit）
    * 标准排除
  * 唯一余数（Naked Single）
    * 同区剩余（Full House）
    * 标准唯一余数
* 区块（Locked Candidates，LC）
  * 宫区块（Pointing）
  * 行列区块（Claiming）
* 数组（Subsets）
  * 显性数组（Naked Subsets）
    * 死锁数组（Locked Subsets）
    * 区块数组（Naked Subsets (+)）
    * 标准显性数组
  * 隐性数组（Hidden Subsets）
* 鱼（Fishes）
  * 普通鱼（Normal Fish）
  * 复杂鱼（Complex Fish）
    * 宫内鱼（Franken Fish）
    * 交叉鱼（Mutant Fish）
* 短链类结构（Wings）
  * 规则 Wing（Regular Wings）
    * XY-Wing
    * XYZ-Wing
    * WXYZ-Wing
    * VWXYZ-Wing
  * 不规则 Wing（Irregular Wings）
    * W-Wing
* 致命结构（Uniqueness）
  * 唯一矩形（Unique Rectangle，UR）
    * 基本类型
      * 类型 1
      * 类型 2
      * 类型 3
      * 类型 4
      * 类型 5
      * 类型 6
      * 隐性唯一矩形
    * 唯一矩形 + 结构
      * 唯一矩形 + 单元格
        * 唯一矩形 + 2D
        * 唯一矩形 + 3X
      * 唯一矩形 + 共轭对
        * 唯一矩形 + 2B / 1SL
        * 唯一矩形 + 2D / 1SL
        * 唯一矩形 + 3X / 2SL
        * 唯一矩形 + 3N / 2SL
        * 唯一矩形 + 3U / 2SL
        * 唯一矩形 + 3E / 2SL
        * 唯一矩形 + 4X / 3SL
        * 唯一矩形 + 4C / 3SL
      * 唯一矩形 + Wings
        * 唯一矩形 + XY-Wing（Unique Rectangle XY-Wing，UR-XY-Wing）
        * 唯一矩形 + XYZ-Wing（Unique Rectangle XYZ-Wing，UR-XYZ-Wing）
        * 唯一矩形 + WXYZ-Wing（Unique Rectangle WXYZ-Wing，UR-WXYZ-Wing）
      * 唯一矩形 + 融合待定数组（Unique Rectangle Sue de Coq，UR-SdC）
  * 可规避矩形（Avoidable Rectangle，AR）
    * 基本类型
      * 类型 1
      * 类型 2
      * 类型 3
      * 类型 5
      * 隐性可规避矩形
    * 可规避矩形 + 结构
      * 可规避矩形 + 单元格
        * 可规避矩形 + 2D
        * 可规避矩形 + 3X
      * 可规避矩形 + Wings
        * 可规避矩形 + XY-Wing（Avoidable Rectangle XY-Wing，AR-XY-Wing）
        * 可规避矩形 + XYZ-Wing（Avoidable Rectangle XYZ-Wing，AR-XYZ-Wing）
        * 可规避矩形 + WXYZ-Wing（Avoidable Rectangle WXYZ-Wing，AR-WXYZ-Wing）
      * 可规避矩形 + 融合待定数组（Avoidable Rectangle Sue de Coq，AR-SdC）
  * 唯一环（Unique Loop，UL）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
  * 全双值格致死解法（Bi-value Universal Grave，BUG）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
    * 全双值格致死解法-双强链法则（Bi-value Universal Grave XZ-Rule，BUG-XZ）
    * 全双值格致死解法 + n（Bi-value Universal Grave Multiple，BUG + n）
    * 全双值格致死解法 + n，带强制链（Bi-value Universal Grave Multiple，BUG + n (+)）
  * 探长致命结构（Borescoper's Deadly Pattern，BDP）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
  * 淑芬致命结构（Qiu's Deadly Pattern，QDP）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
    * 死锁淑芬致命结构（Locked Qiu's Deadly Pattern，Locked QDP）
  * 唯一矩阵（Unique Square，US）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
  * 反转唯一矩形/唯一环（Reverse Bi-value Universal Grave，Reverse BUG）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
* 同数链式结构（Single-digit Patterns，SDP）
  * 多宝鱼（Turbot Fish）
    * 摩天楼（Skyscraper）
    * 双线风筝（Two-string Kite）
    * 标准多宝鱼
  * 空矩形（Empty Rectangle）
  * 守护者（Broken Wings/Guardians）
* 待定数组（Almost Locked Sets，ALS）
  * 欠一数组（Almost Locked Candidates，ALC）
  * 对交空矩形（Empty Rectangle Intersection Pair，ERIP）
  * 假数组/伪数组（Extended Subset Principle，ESP）
  * 融合待定数组（Sue de Coq，SdC）
    * 标准融合待定数组
    * 三维融合待定数组（3D Sue de Coq，3D SdC）
  * 链式待定数组
    * 待定数组-双强链（Almost Locked Sets XZ-Rule，ALS-XZ）
    * 待定数组-XY-Wing（Almost Locked Sets XY-Wing，ALS-XY-Wing）
    * 待定数组-W-Wing（Almost Locked Sets W-Wing，ALS-W-Wing）
    * 死亡绽放（Death Blossom，DB）
* 链（Chains）
  * 普通链（Alternating Inference Chain，AIC）
    * 标准普通链
    * 连续环（Continuous Nice Loop，CNL）
  * 强制链（Forcing Chains，FC）
    * 单元格强制链（Cell Forcing Chains，Cell FC）
    * 区强制链（Region Forcing Chains，Region FC）
  * 动态强制链（Dynamic Forcing Chains，Dynamic FC）
    * 动态单元格强制链（Dynamic Cell Forcing Chains，Dynamic Cell FC）
    * 动态区强制链（Dynamic Region Forcing Chains，Dynamic Region FC）
    * 动态矛盾强制链（Dynamic Contradiction Forcing Chains，Dynamic Contradiction FC）
    * 动态双射矛盾强制链（Dynamic Double Forcing Chains，Dynamic Double FC）
* 飞鱼导弹（Exocets）
  * 初级飞鱼导弹（Junior Exocet，JE）
  * 高级飞鱼导弹（Senior Exocet，SE）
* 广义数组（Generalized Subsets）
  * 多米诺环（Domino Loop/Stephen Kurzhal's Loop）
  * 网（Multi-sector Locked Sets，MSLS）
* 爆破技巧（Last Resorts）
  * 图案叠加删减（Pattern Overlay Method，POM）
  * 模板（Template）
    * 模板填数（Template Set）
    * 模板删数（Template Delete）
  * 人工试数（Bowman's Bingo）
  * 计算机试数（Brute Force）
* 对称性技巧（Symmetricals）
  * 宇宙法（Gurth's Symmetrical Placement，GSP）
  * 高阶宇宙法（Advanced Gurth's Symmetrical Placement，Advanced GSP）
* 杂项（Miscellaneous）
  * 死环（Bi-value Oddagon）



## 难度级别参照表

下面陈列上述技巧的难度分类级别。

| 技巧类别         | 技巧难度级别 |
| ---------------- | ------------ |
| **直观技巧**     | 容易         |
| **区块**         | 一般         |
| **数组**         | 一般         |
| **鱼**           | 困难         |
| **短链类结构**   | 困难         |
| **致命结构**     | 困难 - 极难  |
| **同数链式结构** | 困难 - 极难  |
| **待定数组**     | 困难 - 地狱  |
| **链**           | 极难 - 地狱  |
| **飞鱼导弹**     | 地狱         |
| **广义数组**     | 地狱         |
| **爆破技巧**     | 地狱         |
| **对称性技巧**   | 极难         |
| **杂项**         | 极难         |



## 其它算法实现

该程序除了实现了人工解题算法外，还实现了一部分的经典解题算法。它们是：

* 经典回溯（Backtracking）
* LINQ 算法（LINQing）
* 周运栋位运算算法（Zhou's Bitwise Algorithm）