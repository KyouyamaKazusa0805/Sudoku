---
description: Incomplete Wings
---

# 残缺的分支匹配

前面的内容里，我们介绍了分支匹配逻辑的基本推理思路，是排列各种填数组合并，得到一个或一组删数结论。

不过，我们在最开始的两个技巧 XY-Wing 和 XYZ-Wing 里看出，XYZ-Wing 以及后面的 WXYZ-Wing、VWXYZ-Wing 都是在往上追加单元格。但这一点和最开始 XY-Wing 变为 XYZ-Wing 的方式不同。XY-Wing 只加了一个候选数就变为了 XYZ-Wing。

虽然我们知道这是历史原因所致，但倘若我们继续以这种形式，在 WXYZ-WIng 和 VWXYZ-Wing 的推理上“做手脚”的话，是否就有另外的结构了呢？

## 残缺 WXYZ-Wing（Incomplete WXYZ-Wing） <a href="#incomplete-wxyz-wing" id="incomplete-wxyz-wing"></a>

<figure><img src="../../.gitbook/assets/image.png" alt="" width="375"><figcaption><p>残缺 WXYZ-Wing</p></figcaption></figure>

如图所示。这是一个 WXYZ-Wing 吗？还是 XYZ-Wing 呢？看起来 WXYZ-Wing 的话，好像 `r4c2` 上缺少了一个候选数 3；那是 XYZ-Wing 吗？好像也不是，因为格子已经不止 3 个了。

不过，我们按之前 WXYZ-Wing 的思路推理这个，应该也可以得到结论。假设 `r4c2` 的各种填数情况：

* 如果 `r4c2 = 4`，则和它同宫的 `r6c1 = 3`；
* 如果 `r4c2 = 5`，则和它同宫的 `r6c3 = 3`；
* 如果 `r4c2 = 8`，则和它同行的 `r4c4 = 3`。

显然，由于这个看起来缺了个数的 WXYZ-Wing 仍然具有和 WXYZ-Wing 一致的推理逻辑。倘若我们此时让 `r6c46` 的其中任意一个位置填入 3，只要有一个，它就能同时使得 `r6c13` 以及 `r4c4` 全都填不了 3，这样就产生了矛盾。所以，这个题的结论就是 `r6c46 <> 3`。

我们把分支匹配里初始假设分支的单元格称为**拐点**或**折点**（Pivot）。比如说这个题的拐点是 `r4c2`。然后，我们把这个在拐点上少一个候选数的 WXYZ-Wing 结构称为**残缺 WXYZ-Wing**（Incomplete WXYZ-Wing）。

我们再来看一个例子。

<figure><img src="../../.gitbook/assets/image (1).png" alt="" width="375"><figcaption><p>另一个残缺 WXYZ-Wing</p></figcaption></figure>

如图所示。这个题希望你自己推理。

## 残缺 VWXYZ-Wing（Incomplete VWXYZ-Wing） <a href="#incomplete-vwxyz-wing" id="incomplete-vwxyz-wing"></a>

<figure><img src="../../.gitbook/assets/image (2).png" alt="" width="375"><figcaption><p>残缺 VWXYZ-Wing</p></figcaption></figure>

如图所示。这次我们从 VWXYZ-Wing 里抠掉一个候选数。

这次，我们假设 `r7c4` 的所有的填数情况：

* 如果 `r7c4 = 1`，则和它同宫的 `r9c5 = 6`；
* 如果 `r7c4 = 3`，则和它同宫的 `r9c6 = 6`；
* 如果 `r7c4 = 4`，则和它同列的 `r1c4 = 6`；
* 如果 `r7c4 = 8`，则和它同列的 `r2c4 = 6`。

和前面一样。他们都指向了同一个数字 6。如果此时我们让 `r8c4 = 6`，则会直接导致 `r9c56` 和 `r12c4` 四个单元格都不能填 6 出现矛盾。所以 `r8c4 <> 6` 是这个题的结论。

我们把这个技巧称为**残缺 VWXYZ-Wing**（Incomplete VWXYZ-Wing）。

我们再来看一个例子。

<figure><img src="../../.gitbook/assets/image (3).png" alt="" width="375"><figcaption><p>另一个残缺 VWXYZ-Wing</p></figcaption></figure>

这个例子也希望你自己推理。

那么，分支匹配的内容就全部结束了。下一节我们将进入新的技巧的学习。
