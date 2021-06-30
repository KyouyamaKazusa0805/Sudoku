# 数独解题技巧和算法支持
## 基本技巧支持

目前程序支持如下的技巧项：

* [直观技巧](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Singles/SingleStepSearcher.cs)（Direct Methods）
  * 排除（Hidden Single）
    * 同数剩余（Last Digit）
    * 标准排除
  * 唯一余数（Naked Single）
    * 同区剩余（Full House）
    * 标准唯一余数
* [区块](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Intersections/LcStepSearcher.cs)（Locked Candidates，LC）
  * 宫区块（Pointing）
  * 行列区块（Claiming）
* [数组](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Subsets/SubsetStepSearcher.cs)（Subsets）
  * 显性数组（Naked Subsets）
    * 死锁数组（Locked Subsets）
    * 区块数组（Naked Subsets (+)）
    * 标准显性数组
  * 隐性数组（Hidden Subsets）
* 鱼（Fishes）
  * [普通鱼](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Fishes/NormalFishStepSearcher.cs)（Normal Fish）
  * [复杂鱼](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Fishes/ComplexFishStepSearcher.cs)（Complex Fish）
    * 宫内鱼（Franken Fish）
    * 交叉鱼（Mutant Fish）
* 短链类结构（Wings）
  * [规则 Wing](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Wings/Regular/RegularWingStepSearcher.cs)（Regular Wings）
    * XY-Wing
    * XYZ-Wing
    * WXYZ-Wing
    * VWXYZ-Wing
  * [不规则 Wing](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Wings/Irregular/IrregularWingStepSearcher.cs)（Irregular Wings）
    * W-Wing
* 致命结构（Uniqueness）
  * [唯一矩形](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Uniqueness/Rects/UrStepSearcher.cs)（Unique Rectangle，UR）
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
      * 唯一矩形 + 未知数覆盖/袋鼠（Unique Rectangle Unknown Covering，UR-Unknown Covering）
      * 唯一矩形 + 守护者（Unique Rectangle Guardian，UR-Guardian）
  * [可规避矩形](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Uniqueness/Rects/UrStepSearcher.cs)（Avoidable Rectangle，AR）
    * 基本类型
      * 类型 1
      * 类型 2
      * 类型 3
      * 类型 5
      * 隐性可规避矩形
    * 可规避矩形 + 结构
      * 可规避矩形 + 排除
      * 可规避矩形 + 单元格
        * 可规避矩形 + 2D
        * 可规避矩形 + 3X
      * 可规避矩形 + Wings
        * 可规避矩形 + XY-Wing（Avoidable Rectangle XY-Wing，AR-XY-Wing）
        * 可规避矩形 + XYZ-Wing（Avoidable Rectangle XYZ-Wing，AR-XYZ-Wing）
        * 可规避矩形 + WXYZ-Wing（Avoidable Rectangle WXYZ-Wing，AR-WXYZ-Wing）
      * 可规避矩形 + 融合待定数组（Avoidable Rectangle Sue de Coq，AR-SdC）
  * [唯一环](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Uniqueness/Loops/UlStepSearcher.cs)（Unique Loop，UL）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
  * [全双值格致死解法](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Uniqueness/Bugs/BugStepSearcher.cs)（Bi-value Universal Grave，BUG）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
    * 全双值格致死解法-双强链法则（Bi-value Universal Grave XZ-Rule，BUG-XZ）
    * 全双值格致死解法 + n（Bi-value Universal Grave Multiple，BUG + n）
    * 全双值格致死解法 + n，带强制链（Bi-value Universal Grave Multiple，BUG + n (+)）
  * [探长致命结构](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Uniqueness/Polygons/BdpStepSearcher.cs)（Borescoper's Deadly Pattern，BDP）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
  * [淑芬致命结构](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Uniqueness/Qiu/QdpStepSearcher.cs)（Qiu's Deadly Pattern，QDP）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
    * 死锁淑芬致命结构（Locked Qiu's Deadly Pattern，Locked QDP）
  * [唯一矩阵](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Uniqueness/Square/UsStepSearcher.cs)（Unique Square，US）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
  * [反转唯一矩形/唯一环](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Uniqueness/Reversal/ReverseBugStepSearcher.cs)（Reverse Bi-value Universal Grave，Reverse BUG）
    * 类型 1
    * 类型 2
    * 类型 3
    * 类型 4
* 同数链式结构（Single-digit Patterns，SDP）
  * [多宝鱼](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Sdps/TwoStrongLinksStepSearcher.cs)（Turbot Fish）
    * 摩天楼（Skyscraper）
    * 双线风筝（Two-string Kite）
    * 标准多宝鱼
  * [空矩形](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Sdps/ErStepSearcher.cs)（Empty Rectangle）
  * [守护者](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Sdps/GuardianStepSearcher.cs)（Broken Wings/Guardians）
* 待定数组（Almost Locked Sets，ALS）
  * [欠一数组](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Intersections/AlcStepSearcher.cs)（Almost Locked Candidates，ALC）
  * [对交空矩形](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Alses/EripStepSearcher.cs)（Empty Rectangle Intersection Pair，ERIP）
  * [假数组/伪数组](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Alses/AlsXzStepSearcher.cs)（Extended Subset Principle，ESP）
  * 融合待定数组（Sue de Coq，SdC）
    * [标准融合待定数组](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/RankTheory/SdcStepSearcher.cs)
    * [自噬融合待定数组](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/RankTheory/SdcStepSearcher.cs)（Cannibalistic Sue de Coq，Cannibalistic SdC）
    * [三维融合待定数组](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/RankTheory/Sdc3dStepSearcher.cs)（3D Sue de Coq，3D SdC）
  * 链式待定数组
    * [待定数组-双强链](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Alses/AlsXzStepSearcher.cs)（Almost Locked Sets XZ-Rule，ALS-XZ）
    * [待定数组-XY-Wing](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Alses/AlsXyWingStepSearcher.cs)（Almost Locked Sets XY-Wing，ALS-XY-Wing）
    * [待定数组-W-Wing](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Alses/AlsWWingStepSearcher.cs)（Almost Locked Sets W-Wing，ALS-W-Wing）
    * [死亡绽放](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Alses/DbStepSearcher.cs)（Death Blossom，DB）
* 链（Chains）
  * [普通链](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Chaining/AicStepSearcher.cs)（Alternating Inference Chain，AIC）
    * 标准普通链
    * 连续环（Continuous Nice Loop，CNL）
  * [强制链](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Chaining/FcStepSearcher.cs)（Forcing Chains，FC）
    * 单元格强制链（Cell Forcing Chains，Cell FC）
    * 区强制链（Region Forcing Chains，Region FC）
  * [动态强制链](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Chaining/FcStepSearcher.cs)（Dynamic Forcing Chains，Dynamic FC）
    * 动态单元格强制链（Dynamic Cell Forcing Chains，Dynamic Cell FC）
    * 动态区强制链（Dynamic Region Forcing Chains，Dynamic Region FC）
    * 动态矛盾强制链（Dynamic Contradiction Forcing Chains，Dynamic Contradiction FC）
    * 动态双射矛盾强制链（Dynamic Double Forcing Chains，Dynamic Double FC）
* 飞鱼导弹（Exocets）
  * [初级飞鱼导弹](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Exocets/JeStepSearcher.cs)（Junior Exocet，JE）
  * [高级飞鱼导弹](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Exocets/SeStepSearcher.cs)（Senior Exocet，SE）
* 广义数组（Generalized Subsets）
  * [多米诺环](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/RankTheory/SkLoopStepSearcher.cs)（Domino Loop/Stephen Kurzhal's Loop）
  * [网](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/RankTheory/MslsStepSearcher.cs)（Multi-sector Locked Sets，MSLS）
* 爆破技巧（Last Resorts）
  * [图案叠加删减](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/LastResorts/PomStepSearcher.cs)（Pattern Overlay Method，POM）
  * [模板](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/LastResorts/TemplateStepSearcher.cs)（Template）
    * 模板填数（Template Set）
    * 模板删数（Template Delete）
  * [人工试数](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/LastResorts/BowmanBingoStepSearcher.cs)（Bowman's Bingo）
  * [计算机试数](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/LastResorts/BfStepSearcher.cs)（Brute Force）
* 对称性技巧（Symmetricals）
  * [宇宙法](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Symmetry/GspStepSearcher.cs)（Gurth's Symmetrical Placement，GSP）
  * [高阶宇宙法](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/Symmetry/Gsp2StepSearcher.cs)（Advanced Gurth's Symmetrical Placement，Advanced GSP）
* 杂项（Miscellaneous）
  * [死环](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/Manual/RankTheory/BivalueOddagonStepSearcher.cs)（Bi-value Oddagon）



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

* [经典回溯](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/BruteForces/BacktrackingSolver.cs)（Backtracking）
* [LINQ 算法](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/BruteForces/OneLineLinqSolver.cs)（LINQing）
* [周运栋位运算算法](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Solving/BruteForces/UnsafeBitwiseSolver.cs)（Zhou's Bitwise Algorithm）