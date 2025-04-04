﻿---
description: Size-Extended Unique Loop
---

# 唯一环的规格推广

前文的内容我们已经针对唯一环的基本逻辑进行了描述，下面我们来看看规格不是 6 个单元格的其他情况。

## 8 个单元格的唯一环 <a href="#size-8" id="size-8"></a>

<figure><img src="../../.gitbook/assets/images_0201.png" alt="" width="375"><figcaption><p>8 格唯一环</p></figcaption></figure>

如图所示。我们发现，整个结构里只有 `r9c4` 包含额外的候选数 4。如果我们让它从盘面里消失的话，所有图中的 8 个单元格将只剩下 7 和 8。

我们试着套用之前给的规则，我们发现这 8 个单元格也满足。所以它是唯一环，能够引发矛盾。因此结论就是 `r9c4 <> 78` 了。和唯一矩形类似，这个地方也可以认为是 `r9c4 = 4`，但如果这个单元格包含其他的数字后，我们就只能认为他们其中一个是成立的，因此延续之前的说法，我们这里也统一给出的结论是删掉 7 和 8，而不是填 4。

## 10 个单元格的唯一环 <a href="#size-10" id="size-10"></a>

下面我们来看 10 个单元格的情况。

<figure><img src="../../.gitbook/assets/images_0013.png" alt="" width="375"><figcaption><p>10 格唯一环</p></figcaption></figure>

如图所示。这里我们用的是类型 4 的思维。

我们发现，`b2` 里能填入 4 的只有 `r1c5` 和 `r3c6` 两个单元格。如果我们再让他们俩里剩下的那个单元格填 9 的话，那么 4 和 9 就只出现在其中，于是和结构其他的 8 个单元格一共凑满 10 个单元格。

按照之前介绍的唯一环的形成条件来看，整个 10 个单元格是可以满足唯一环的条件的。所以，它会形成唯一环那样的矛盾。因此，最初我们假设让 `r1c5` 或 `r3c6` 里填一处 9 就会直接引发矛盾。

因此，这个题的结论就是 `{r1c5, r3c6} <> 9`。

## 12 个单元格的唯一环 <a href="#size-12" id="size-12"></a>

<figure><img src="../../.gitbook/assets/images_0084.png" alt="" width="375"><figcaption><p>12 格唯一环</p></figcaption></figure>

如图所示。这里我们用的是类型 2 的思维。

如果 `r1c3`、`r2c4` 和 `r5c2` 全部把候选数 1 去掉，那么图中结构用到的一共 12 个单元格会形成唯一环引发矛盾。

所以，这三个 1 不能全部同时去掉。反过来说就是，这三个 1 至少有一个是对的数字。那么，`r2c2 = 1` 会同时引起刚才得到的那三个 1 全部清除，所以 `r2c2 <> 1` 是这个题的结论。

## 14 个单元格的唯一环 <a href="#size-14" id="size-14"></a>

<figure><img src="../../.gitbook/assets/images_0112.png" alt="" width="375"><figcaption><p>14 格唯一环</p></figcaption></figure>

如图所示。这次我们用类型 3 的思维来看这个题。

由于图中 `r89c3` 里额外的候选数 4、7、9 不能同时去掉（会引发唯一环的矛盾），所以我们要确保里面剩下至少一个候选数填入到这两个单元格里。不过我们还发现，`c3` 里已经存在三个单元格只剩下 2、4、7、9 四种不同的候选数。那么我们刚好可以使得 `r89c3` 拿出一个数与他们配成一个显性四数组。所以，`c3` 其他单元格都不能是 2、4、7、9。所以这个题的结论是 `r5c3 <> 4`。

## 唯一环的最大规格是多少？ <a href="#max-size-of-unique-loop" id="max-size-of-unique-loop"></a>

唯一环肯定存在一个最大规格。那么，它应该是多少呢？18？16？

答案是 14。首先，我们要让一个唯一环在整个盘面里放上，以满足唯一环的形成条件，那么每一个宫最多只能用两个单元格。所以，粗糙地估计，唯一环最大是不能超过 18 的，毕竟就 9 个宫嘛。

但是实际上的是，18 是肯定不行的。一旦一个唯一环用到 18 个单元格，就直接说明这个题具有两个解了，毕竟就算其他数字我们不关心，但这两个数字内部直接可以随便换。

那么 16 呢？16 其实也是不行的。16 个单元格的唯一环意味着你要用到 8 个宫。那么最后剩下的那个宫势必会直接出现无法填数的矛盾。

为什么这么说？这里似乎有些难以理解。

让我们凭空想一下。在宫排除技巧里，我们知道一个潜在的特征：每一个宫都有 4 个和他并排同一个“大行列”的宫。比如说和 `b1` 并排的 4 个宫分别是 `b2347`。说宫排除是因为，我们假设要在 `b1` 内填数了，那么能够正常影响 `b1` 使得它出数的排除项必须落在 `b2347` 里，只有他们才能有机会影响到 `b1`。对于唯一环技巧而言，说这个的目的是让你明白，一旦一个盘面其中 8 个宫都用到，那么唯一没用到的那个宫势必和它并排的那四个宫全部是被唯一环用了。

但是，按照排除法的逻辑，四个并排的宫造成的排除效果，会同时让最后剩下的那个宫必定可以根据宫排除同时得到两个数的出数位置。但是问题就在这里。因为唯一环的摆放关系，最后引发的出数一定会落在同一个位置上。如果不落在同一个位置上的话，外部的唯一环要么就形不成，要么规格就不是 16，因为这俩的位置也会纳入其中。

如果你凭空想象不出来的话，那么我们来看 `b5` 其中一种能引发上述说法的唯一环的局部摆放：

<figure><img src="../../.gitbook/assets/images_0136.png" alt="" width="375"><figcaption><p>一个能引起特殊矛盾的 16 个单元格唯一环</p></figcaption></figure>

如图所示。这是一个我随意构造的唯一环，用到 16 个单元格。之所以说是“随意构造的”，主要是为了解释我要说的矛盾的点，唯一环的连线反倒不是特别重要，只是怕你觉得这个大结构看不出来怎么连的，我才标出来的（不过是随意找了其中一个路径罢了）。可以看出，`b5` 的上下左右四个宫都会被唯一环所用，按照涂色不同来区分填数的不同，我们可以看到，绿色在 `b2468` 里有四处，而橙色也有四处。

虽然他们摆放位置不同，但是因为唯一环的原因，摆放特殊性会使得 `b5` 里，代表绿色和橙色的那两个填数因为排除法的关系会直接让 `r5c5` 同时填绿色和橙色数字，这显然就违背了数独规则。

这便是我要说的点——唯一环要用到 8 个宫的话，就会让外部摆放的数字引发两种数字的宫排除，其结论会使得不同的填数填入到同一个单元格上，引发这样的矛盾。

当然，你可能还是不满足于我这个说辞（毕竟哪哪看着都像是凑出来的例子）。那你不妨仔细品鉴一下，构造一些例子来对此进行严格的数学证明。不过这里就省略了。

所以，唯一环的最大规格是 14。

至此，我们就把唯一环的内容全部结束了。更多的例子还希望你自己去探索。这个技巧出现频率其实并不算高，因为规格的关系，它找起来的复杂程度确实很高，使得平时做题的时候很难遇到它。不过，在这么多例子的加持下，我相信你对唯一环的内容已经有了一个充分的认识。下面我们将进入到另一个技巧的学习。
