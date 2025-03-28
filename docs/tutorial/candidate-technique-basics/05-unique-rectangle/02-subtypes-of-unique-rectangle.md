﻿---
description: Subtypes of Unique Rectangle
---

# 唯一矩形的类型

之前我们介绍到唯一矩形的基本推理思路和原理。由于唯一矩形的思维独立于我们之前学到的任何一个数独技巧，所以它可以搭配之前学过的技巧的推理方式产生不同的思路。我们把这些唯一矩形都进行了类型的归纳。

按照约定俗成的规矩，我们把之前介绍的那个基础推理过程归为**类型 1**（Type 1）。那么本文将为大家介绍后续的一共 6 种新类型的推理逻辑，先从**类型 2**（Type 2）开始。

## 类型 2（Type 2） <a href="#type-2" id="type-2"></a>

<figure><img src="../../.gitbook/assets/images_0089.png" alt="" width="375"><figcaption><p>类型 2</p></figcaption></figure>

如图所示。这个例子就比之前介绍的类型 1 多了一个候选数。不过为了强调推理过程，我换了个颜色把多出来的候选数标注了出来。

如果这两个橘色的 5 从盘面里消失，则 `r38c13` 四个单元格将只剩下 3 和 9。按照之前介绍的思路，由于它会造成唯一矩形的矛盾，因此这种结构应该避免，所以假设就不成立了。

因为我们假设的是 `r8c13 <> 5` 才会导致不成立，所以它的相反情况则是 `r8c13` 里必须有一个是 5。换言之，这两个单元格就会构成一个关于数字 5 的区块结构。所以，所在行和所在宫的其他任何一个单元格都不能填入 5，因为这行和这个宫里填 5 的机会必须从 `r8c13` 里诞生。

所以，这个题的结论就是 `{r7c3, r8c7, r9c1} <> 5`。

我们把这种推理过程称为类型 2，即第二种唯一矩形的推理思路；另外，由于它会用到区块的思路，所以也经常被称为**区块类型**。

## 类型 3（Type 3） <a href="#type-3" id="type-3"></a>

既然有区块，那肯定不能少了数组。我们把使用数组推理的唯一矩形称为**类型 3**（Type 3），也叫**数组类型**。不过数组类型的思路会稍微复杂不少，因为数组有规格一说，所以这个部分会多给大家展示一些例子。

### 显性数组 <a href="#with-naked-subset" id="with-naked-subset"></a>

数组分显隐性，那么唯一矩形里用的时候也分显隐性两种。先来看显性的。

#### 显性数对 <a href="#with-naked-pair" id="with-naked-pair"></a>

<figure><img src="../../.gitbook/assets/images_0011.png" alt="" width="375"><figcaption><p>类型 3，带显性数对</p></figcaption></figure>

如图所示，请仔细观察 `r7`。这次和类型 2 又有不同——多出来的两个候选数甚至都不是同一个数字了。不过也没事，推理肯定是可以继续的。

我们优先看到，`r7` 上还有一个空格 `r7c5` 只有 6 和 7，刚好和我们唯一矩形四个格子里多出来的两个候选数是一样的数字。如果我们安排 `r7c5` 填一个数字 6 或 7 进去，由于 `r7c23` 里的 6 和 7 不能同时被去掉（造成唯一矩形的矛盾），所以里面还得再出一个 6 或者 7。虽然我们无法确保 6 和 7 的组合关系究竟如何，但我们可从前面给予的判断条件里知道，`r7` 最终会在 `r7c23` 里选一个单元格，和 `r7c5` 构成关于 6 和 7 的显性数对。

> 你别看 `r7c23` 里的 6 和 7 都只剩下一个候选数了，而标准的显性数对至少需要每个单元格都有两个候选数。但是仔细想想它似乎也没啥毛病：毕竟 `r7c23` 本身哪一个单元格是 6 或 7 我们都确定不了，所以剩下一个候选数也不会有什么问题。所以这里我们只是美其名曰按显性数对的方式给这个结构进行了命名。它当然不是标准的显性数对，只是看起来像，就这么叫它。

我们知道它会在 `r7` 里构成关于 6 和 7 的显性数对就行了，构成的单元格是 `r7c235` 里的其中两个单元格。因为这一行有显性数对，按照显性数对的删数逻辑，它应该删除的是所在行的其他单元格里的 6 和 7，因此本题的结论是 `r7c8 <> 7`，它是这个题里唯一的一个删数。

#### 显性三数组 <a href="#with-naked-triple" id="with-naked-triple"></a>

<figure><img src="../../.gitbook/assets/images_0082.png" alt="" width="375"><figcaption><p>类型 3，带显性三数组</p></figcaption></figure>

如图所示。既然显性数对要用三个单元格的其中两个，那么显性三数组自然就是四个单元格的其中三个。

可以看到，在 `c4` 里有两个单元格总体只能填 1、2、7 三种数字。此时我们发现，这个所谓的 1、2、7，其实也是 `r46c4` 里唯一矩形多出来的候选数。倘若我们让 `r46c4` 不填 1 也不填 7，唯一矩形这四个单元格就会出现矛盾。因此，肯定是需要有 1 或 7 的填入的。

确保这一点后，我们就可以发现，`r59c4` 里只能是 1、2、7 的其二，而 `r46c4` 又需要保证有 1 和 7 的填入，所以呢？所以 `r46c4` 里就会拿出一个单元格填入 1 或者 7，来和 `r59c4` 配成一个整体关于 1、2、7 的显性三数组结构。因此，所在列上的其他任何的单元格都不能填入 1、2、7，于是我们就有结论 `r8c4 <> 1`。

#### 显性四数组 <a href="#with-naked-quadruple" id="with-naked-quadruple"></a>

<figure><img src="../../.gitbook/assets/images_0110.png" alt="" width="375"><figcaption><p>类型 3，带显性四数组</p></figcaption></figure>

如图所示。这次继续把规格推广一个单元格。和前面的思路完全一样，因为我们要保证 `r5c12` 里必须有一个 1 或 7 的填入，而 `r5` 里有三个单元格总体只能填 1、2、6、7，于是他们填 1、2、6、7 的其三，就直接和 `r5c12` 里出现的 1 或者 7 配在一起，形成显性四数组。

因此，`r5` 里的其他空格就不能填 1、2、6、7 了。对于这个题，结论就是 `r5c8 <> 27`。

好了。显性数组的部分就说完了。下面我们来介绍隐性数组的部分。

### 隐性数组 <a href="#with-hidden-subset" id="with-hidden-subset"></a>

隐性数组的思路会稍微绕一些。所以不要走神。

#### 隐性数对 <a href="#with-hidden-pair" id="with-hidden-pair"></a>

<figure><img src="../../.gitbook/assets/images_0134.png" alt="" width="375"><figcaption><p>类型 3，带隐性数对</p></figcaption></figure>

如图所示。这回我们把思路倒过来。我们看到，`b2` 里数字 1 和 2 只能填在 `r123c6` 三个单元格之中。由于我们不能同时让 `r12c6` 直接是 1 和 2（唯一矩形的矛盾），所以 `r12c6` 里只能二选一。选出来后，和 `r3c6` 就形成了关于 1 和 2 的隐性数对了。

> 有人问，`r12c6` 必须二选一吗？我能都不选吗？看起来唯一矩形的矛盾在都不选的时候也可以避免啊。但是实际上肯定是不能的。因为虽然唯一矩形不矛盾了，但是数字 1 和 2 必须在这三个单元格里选两个单元格填进去，所以你必须确保有两个位置填。都不选就意味着 1 和 2 填不满 `b2`，这直接就矛盾了。
>
> 这里也是和显性数组一样，实际上 `r12c6` 也是确定不了究竟谁是是被选中的格子，但是因为思路非常像是隐性数对，所以美其名曰也叫它隐性数对。

因为有隐性数对的存在，所以 `r12c6` 里有一个格子是 1 或 2，配合 `r3c6` 构成隐性数对。我们能确定的只有 `r3c6`，它必须填 1 或 2。所以，`r3c6 <> 34` 就是这个题的结论了。

这就是带有隐性数组的推理逻辑了。后面的隐性三数组和隐性四数组思路都是大体一致。下面我们来看看。

#### 隐性三数组 <a href="#with-hidden-triple" id="with-hidden-triple"></a>

<figure><img src="../../.gitbook/assets/images_0157.png" alt="" width="375"><figcaption><p>类型 3，带隐性三数组</p></figcaption></figure>

如图所示，这个题用到了隐性三数组。

看 `c4` 可以发现，数字 1、3、8 整体只能填入到 `r2345c4` 四个单元格里。由于 `r45c4` 里只能有一个格子填 1 或 8，所以 1、3、8 的填数机会还剩下两个单元格；而刚好还剩下两个单元格 `r23c4`。于是，隐性三数组就形成了，即 `r23c4` 里填 1、3、8 的其二，配合 `r45c4` 里选取其中一个格子填上剩下的那个数，三个格子填 1、3、8 三种数字就占满了。

于是，这列其他单元格都不能填 1、3、8，于是我们可以去掉他们。所以这个题的结论就是 `r23c4 <> 9`。

#### 隐性四数组 <a href="#with-hidden-quadruple" id="with-hidden-quadruple"></a>

<figure><img src="../../.gitbook/assets/images_0180.png" alt="" width="375"><figcaption><p>类型 3，带隐性四数组</p></figcaption></figure>

如图所示。这个题也是一样。因为 6 和 7 在 `r7c89` 里只能出一个，而 4、6、7、8 四个数字整体在 `r7` 里只能出现在 `r7c12389` 里，所以还剩下三个单元格。刚好三个单元格加 `r7c89` 其一就把四个数的填数机会用完了。因此，这四个单元格（`r7c123` 和 `r7c89` 的其一）形成关于 4、6、7、8 的隐性四数组。所以，这个题的结论就是 `r7c1 <> 39, r7c2 <> 1, r7c3 <> 9` 了。

至此，隐性数组的部分也全部讲完了。

### 类型 3 里的显隐性互补 <a href="#law-of-complement-in-type-3" id="law-of-complement-in-type-3"></a>

不知道你有没有细看这些例子。在唯一矩形里用到的数组其实也是具备显隐性互补的特征的，不过因为它中间用到的空格有一小块是不定的，所以不如直接的数组看起来互补性那么直观。

因为数组一定是有互补的，所以我们就拿前面的其中一个题来给各位介绍一下类型 3 里的互补就行了。其他的就自己看吧。

<figure><img src="../../.gitbook/assets/images_0199.png" alt=""><figcaption><p>类型 3 的显隐性互补示例</p></figcaption></figure>

如图所示。这是同一个题目，来自于前面的隐性四数组那个题，只是换了一个删数的位置。

可以看到，左边的是关于 1 和 3 的显性数对，右边的是关于 6 和 7 的隐性数对，虽然数字用的不一样，但是结论是一样的，都是 `r9c7 <> 13`。

类型 3 的显隐性互补，你需要先确定显性数组和隐性数组都有哪些单元格是一定固定不动的，然后互补的时候去改变他们的位置。举个例子，对于显性数组来说，因为 `r7c89` 是不确定的，所以固定不变的只有 `r8c7`。隐性数组这边则是 `r9c7` 不变。当需要找互补的数组时，只需要把这个宫（这个例子是宫，别的例子不一定是宫，可以是行或者列，具体情况具体讨论即可）的这部分格子换一下，别的位置都不变，那么我们就能得到另外互补的那一边的数组。这个题因为宫内只有四个空格，除去唯一矩形用了两个单元格，就还剩下两个单元格了，互补过去就只剩一个单元格，所以就是 `r8c7` 和 `r9c7` 他们俩互补。

所有其他的例子也都是一样的道理，这里就不赘述了。有兴趣的话可以自己看看。因为这种互补也是有效的，所以在类型 3 里数组的规格在实际使用上也不会超过 4 个单元格，所以也不存在五数组这种说法，因为也必有互补的另一边比五数组规格更小。

## 类型 4（Type 4） <a href="#type-4" id="type-4"></a>

总算是来到**类型 4**（Type 4）了。类型 4 虽然不如类型 3 有那么多种规格不同的情况（它就一种情况），但是它理解起来也不容易。

<figure><img src="../../.gitbook/assets/images_0217.png" alt="" width="375"><figcaption><p>类型 4</p></figcaption></figure>

如图所示。我们这次甚至都不要关心多出来几个候选数。我们发现，`r5` 里，数字 2 只能填在唯一矩形用到的 `r5c15` 里。这非常危险：因为 `r5c15` 里必须有一个 2，而上面的 `r4c15` 肯定是 2 和 7 了，所以不管怎么排列 2 和 7 的填数，`r5c15` 都不敢再往其中填 7 了，不然四个格子只有 2 和 7 将直接形成唯一矩形的矛盾。

所以，这个题的结论就是 `r5c15 <> 7`，两边的候选数 7 均需要删除，毕竟 `r5c15` 的随便哪一边是 7，另外一边都必须是 2，因为 `r5` 只能这两个单元格填 2。

我们把这个类型称为类型 4。它用到了一个全新的概念：**共轭对**（Conjugate Pair）。简单来说，共轭对指的是这个 2 只能填入在 `r5c15` 其中，导致“非此即彼”的这种状态。因此，我们把这个类型 4 也称为**共轭对类型**。

在以后的内容里，我们还会学到链技巧，链的概念里会出现一个术语叫“强链”，从使用上来讲，是共轭对的超集，即所有的共轭对都是强链的最常见的形态，所以共轭对类型也常被称为**强链类型**。但是因为这里使用的术语超纲了，所以我们就不拓展这部分的内容了，还是暂时使用类型 4 或共轭对类型称呼此技巧。

## 类型 5（Type 5） <a href="#type-5" id="type-5"></a>

前面介绍了类型 2、3、4 几乎已经是全部常见用法了。下面给各位介绍的是针对于前面类型 2、3、4 本身推理上的推广。

<figure><img src="../../.gitbook/assets/images_0232.png" alt="" width="375"><figcaption><p>类型 5</p></figcaption></figure>

如图所示。它是类型 2 的推广。类型 2 里有两个单元格里多出同一个数字，因此他们会构成区块。这次有三个单元格都多出一个数字，因此结论就是他们仨不能同时从盘面里消失。

既然这三个候选数 3 都不能同时消失的话，那么就说明他们里面至少有一个是对的。注意，这次只能说至少有一个了。因为我们很明显可以看出来，`r3c1` 和 `r9c3` 是可以同时填 3 的，也没有明显的矛盾，但是这并没有什么特殊意义，它并不影响我们后续的推理，这里只用得上“至少一个”这么个说法。

当我们知道至少有一个的时候，那么我们就去找和 XYZ-Wing 那样的删数位置——填一个 3 使得这三处位置全不能填 3。这题还真有，就是这里的 `r8c1(3)` 了。所以，`r8c1 <> 3` 是这个题的结论。

我们把这种推广类型 2 的模式称为**类型 5**。不过，类型 5 也不一定非得是三个单元格构成。因为，就两个单元格多出来一个数字也是 OK 的，只是它得斜着摆，这种情况特别罕见。

<figure><img src="../../.gitbook/assets/images_0019.png" alt="" width="375"><figcaption><p>类型 5，另一种摆放模式</p></figcaption></figure>

如图所示。这种情况也是属于类型 5 的。总而言之，只要让类型 2 里多出来的数字摆放位置存在斜着的情况的时候，就称为类型 5。

## 类型 6（Type 6） <a href="#type-6" id="type-6"></a>

下面我们来看类型 4 的推广。类型 4 一共具有两种推广，不过有一种推广是没有名字的，也就是这里的**类型 6**（Type 6），另外一种有一个单独的技巧名。先来说类型 6 这个没有名字的情况。

<figure><img src="../../.gitbook/assets/images_0033.png" alt="" width="375"><figcaption><p>类型 6</p></figcaption></figure>

如图所示。类型 6 是把共轭对类型的一个共轭对推广成了两个。因为这个思路稍微有些不够直接，所以需要严格地梳理一下逻辑。

我们可以看到，数字 4 有两处共轭对，且恰好位于 `r46` 上，还恰好两个共轭对用到的格子刚好就是唯一矩形用到的四个单元格。这意味着，这四个单元格里必须出现两个 4。

由于 `r4c7` 和 `r6c3` 只有两个候选数 4 和 9，所以这两个单元格显然填不了别的数字。结合共轭对我们可以得到的信息是，如果 `r4c3` 填 4 或者 `r6c7` 填 4 就会引发唯一矩形的矛盾。举个例子，如果 `r4c3 = 4`，则 `r4c7` 和 `r6c3` 因为只有 4 和 9 的关系，他们俩就只能填 9。于是乎，`r6` 的共轭对的关系会直接导致 4 填在 `r6c7` 里，然后就形成了 4 和 9 的唯一矩形的矛盾；同理，从 `r6c7 = 4` 出发，我们也可以得到 `r6c3` 和 `r4c7` 填 9 的结果，于是，由于 `r4` 的共轭对，导致 `r4c3 = 4`，于是也形成了矛盾。

所以，这个题的结论是 `{r4c3, r6c7} <> 4`。

我们把这种类型称为类型 6。

什么样的题属于类型 6 呢？两个共轭对，用到同一个数字，且构成平行的状态。我们再来看一个例子。

<figure><img src="../../.gitbook/assets/images_0046.png" alt="" width="375"><figcaption><p>类型 6，另一个例子</p></figcaption></figure>

这个题就自己看了。

## 隐性唯一矩形（Hidden Unique Rectangle） <a href="#hidden-unique-rectangle" id="hidden-unique-rectangle"></a>

下面介绍最后一种情况，也是两个共轭对，也是相同的数字的共轭对，但是是垂直的。

<figure><img src="../../.gitbook/assets/images_0050.png" alt="" width="375"><figcaption><p>隐性唯一矩形</p></figcaption></figure>

如图所示，这次共轭对是垂直摆放的，所以有些别扭，因为不方便我们讨论填数情况。这次我们换个思路。

因为两个垂直的共轭对用到了三个不同的候选数 5，所以他在内部的可填情况只有两种：

* `r9c5 = 5`；
* `r7c5 = 5` 和 `r9c7 = 5`。

没有其他情况了。显然，`r7c5` 和 `r9c7` 是必须同时填 5 的，不能只填一个。因为这两个共轭对要求 `c5` 和 `r9` 都得有填 5 的情况，只填一个就会使得其中一个行列上没填 5 直接出错。

当我们确定有这两种情况后，我们可以直接进行推理：

* 如果 `r9c5 = 5`，则因为它填上了数字，所以占位使得 `r9c5 <> 9`；
* 如果 `r7c5` 和 `r9c7` 同时填 5，则因为 `r7c7` 只有 5 和 9 两种候选数，因此它此时必须填 9；而另一边，`r9c5` 如果此时填 9 就会使得四个单元格只有 5 和 9，出现唯一矩形的矛盾，所以也可以得到结论 `r9c5 <> 9`。

显然，不论两种情况的哪一种，我们都可以得到 `r9c5 <> 9`，所以 `r9c5 <> 9` 是正确的结论。

我们把这种摆放是垂直的情况称为**隐性唯一矩形**（Hidden Unique Rectangle，简称 HUR）。

> 隐性唯一矩形在一些地方也称为类型 7，但是因为唯一矩形在类型编号上对于类型 7 是否纳入正式类型并没有一个统一标准，所以这里就不纳入进来了，毕竟它有一个单独的技巧名。

我们再来看一则例子。

<figure><img src="../../.gitbook/assets/images_0054.png" alt="" width="375"><figcaption><p>隐性唯一矩形，另一个例子</p></figcaption></figure>

这个例子就自己推理了。方法大概和前面是一样的，只是位置不一样。

至此，我们就把全部的唯一矩形的类型（有编号的类型）全部介绍完毕了。实际上，唯一矩形的内容并没有完整结束，但是它的一些复杂用法就放在以后说了。
