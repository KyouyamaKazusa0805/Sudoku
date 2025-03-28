﻿---
description: Finned Fish
---

# 鳍鱼

前面我们学到了鱼的基本推理逻辑。下面我们来看看，鱼的一个特殊的部件。

## 二阶鳍鱼（Finned X-Wing） <a href="#finned-x-wing" id="finned-x-wing"></a>

<figure><img src="../../.gitbook/assets/images_0230.png" alt="" width="375"><figcaption><p>二阶鳍鱼</p></figcaption></figure>

如图所示。我们发现，配色比二阶鱼多了一个蓝色。

这一次我们假设 `c67` 里填入的 7 的排列多出了一个单元格：`r1c6`，而它肯定会影响我们二阶鱼的成立。那么我们干脆把它“分离”出去。我们把它当成一个特殊的存在，假设它最终是不是正确的填数，然后看看结论是否有用。

* 如果 `r1c6 = 7`，则按照直观排除的效果，`r1c6` 所在的行、列、宫里的其他空格都不能填 7，删除这些位置的候选数 7；
* 如果 `r1c6 <> 7`，则盘面可以删去这个候选数 7，于是一个标准版的二阶鱼就形成了，删除的结论是 `r24` 的其他空格的候选数 7（推理过程略）。

很显然，这两个毫不相干的结论，是有交叉的部分的，那就是 `r2c45(7)`。第一种情况下，由于 `r1c6 = 7` 会影响到所在宫（即这里的 `b2`），所以 `r2c45 <> 7` 是这个情况下删数的一部分；而第二种情况下，由于鱼成立，所以删数是 `r24` 的别的 7，而 `r2c45` 也恰好位于 `r24` 里属于删除范围的一部分。

显然，`r1c6 = 7` 和 `r1c6 <> 7` 已经是全部的可能性。那么既然两边都能去掉 `r2c45 <> 7`，那么我们可以认为，这个删数结论就是正确的。所以这个题的结论就是 `r2c45 <> 7`。

我们把这个技巧称为**二阶鳍鱼**或**鳍二阶鱼**（Finned X-Wing），当然也可以叫**鳍二链列**，并把影响二阶鱼成立的蓝色数字 7（即图中的 `r1c6(7)`）称为**鱼鳍**（Fin），这样的鱼称为**鳍鱼**（Finned Fish），即带有鱼鳍的鱼。

## 三阶鳍鱼（Finned Swordfish） <a href="#finned-swordfish" id="finned-swordfish"></a>

<figure><img src="../../.gitbook/assets/images_0018.png" alt="" width="375"><figcaption><p>三阶鳍鱼</p></figcaption></figure>

如图所示。请观察 `r189` 这三行的数字 7。

我们将 `r1c6(7)` 当成鱼鳍，假设它的两种可能性：

* 如果 `r1c6 = 7`，则删除的是所在行、列、宫其他位置的 7；
* 如果 `r1c6 <> 7`，则标准的三阶鱼成立，删除 `c348` 其他位置的 7。

显然，和前面的题完全一样的是，可以删除的部分也刚好存在。这个题是 `r3c4(7)`。所以，`r3c4 <> 7` 是这个题的结论。

我们把这个结构称为**三阶鳍鱼**或**鳍三阶鱼**（Finned Swordfish），早期也叫**鳍三链列**。

## 四阶鳍鱼（Finned Jellyfish） <a href="#finned-jellyfish" id="finned-jellyfish"></a>

下面我们来看规格为 4 的情况。

<figure><img src="../../.gitbook/assets/images_0032.png" alt="" width="375"><figcaption><p>四阶鳍鱼</p></figcaption></figure>

如图所示。我们把这个题里的 `r6c7(9)` 视为鱼鳍，假设它的两种情况：

* 如果 `r6c7 = 9`，则删除所在行、列、宫其他单元格里的候选数 9；
* 如果 `r6c7 <> 9`，则四阶鱼成立，删除 `r1489` 其他空格里的候选数 9。

可以看到，两种状态下，`r4c8(9)` 都是覆盖得到的部分，所以 `r4c8 <> 9` 是这个题的结论。

我们把这个结构称为**四阶鳍鱼**或**鳍四阶鱼**（Finned Jellyfish），早期也叫**鳍四链列**。

## 对鳍鱼的一些疑问 <a href="#questions-for-fins" id="questions-for-fins"></a>

### 问题 1：鳍鱼的形成似乎有些…… <a href="#i-wonder-how-a-finned-fish-can-be-formed" id="i-wonder-how-a-finned-fish-can-be-formed"></a>

可以从前面讲解的逻辑上看出，似乎删数的形成有些过于“草率”，它的形成有些摸不着头脑。就是说，我不知道鳍鱼是否存在一个通用的“公式”，我只要找到这个切入条件，鳍鱼的形成就可以自己判断了，而不是光看例子就看得懂，自己找的时候就不知道怎么去看。

我们要明白一个点。我们首先需要拿出鱼鳍来作为删数条件的判断，那么鱼鳍就必须有摆放上的要求。就是说，这个鱼鳍的位置是有讲究的，它肯定不能是任何一个位置上。

可以通过前面展示的例子里看出，它一定处于初始假设的行列上。就是说，它的位置，一定要影响鱼的形成。比如说一个鱼初始排列需要用 `r124` 三行，那么鱼鳍就必须在 `r124` 行上，而不能是比如说 `r9` 这些位置上。

其次，鱼鳍的两种状态要使得删数在取“交集”的时候能有删数，那么它的位置一定要和鱼本身的删除的部分有交集。这说明了鱼鳍的位置肯定也不能放在跟鱼本身没有任何关联的位置。

我们还是拿规格最小的二阶鳍鱼举例。

<figure><img src="../../.gitbook/assets/images_0230.png" alt="" width="375"><figcaption><p>还是前面那个二阶鳍鱼</p></figcaption></figure>

如图所示。这是刚才那个例子。我拿这个例子给各位解释我刚才说的第二点到底是什么意思。我们先忽略 `r1c6(7)` 的存在，这里是为了说明鱼的形成，所以暂时先不要它，当他不在盘面里。

由于要有删数，所以，7 的位置一定就不能距离四个绿色的 7（二阶鱼形成的这四个位置）过远。比如说，假设我在 `r7c6` 这个地方有一个候选数 7。我们把 `r7c6(7)` 视为鱼鳍的话，这个时候的假设就会成这样：

* 如果 `r7c6 = 7`，则 `r7c6` 所在行列宫其他单元格都不能是 7；
* 如果 `r7c6 <> 7`，则二阶鱼成立，删数按最开始 `r24` 其他空格删掉 7 即可。

显然，鱼鳍肯定是影响不到第二点（`r7c6 <> 7` 这种情况）的。那么主要改变的是第一点的逻辑。可是，我们要想在删数上取交集，而 `r7c6(7)` 是鱼鳍的话，这是不可能有交集的。因为根据第二点，删数的范围在 `r24` 上，但鱼鳍距离这两行实在是太远了。不论是 `r7c6` 的行也好，列也好，宫也好，都没有任何和 `r24` 上构成删数的交集部分。

我想说的就是这个意思。要想保证这个鱼鳍能够最终起效并造成有效删数，它势必需要跟鱼本身的结构有些许关系。那么你觉得这个关系是什么呢？

是的！只要让鱼鳍（配色为蓝色的位置）的出现位置跟鱼本身（配色为绿色的那些位置）的任意其中一个格子同宫即可。只要鱼鳍和鱼的某一个单元格同宫，那么这个鱼鳍就是有效的。

有这一点之后，我们回去看所有的例子。是不是就豁然开朗了？所有的三个例子，鱼鳍都和鱼的其中一个格子同一个宫。

### 问题 2：鱼鳍就只能有一个吗？ <a href="#is-the-number-of-fins-must-be-1" id="is-the-number-of-fins-must-be-1"></a>

有了前面问题的铺垫，那么这个问题就有答案了。

显然，鱼鳍在同一个题里不一定非得只有一个。

<figure><img src="../../.gitbook/assets/images_0045.png" alt="" width="375"><figcaption><p>两个鱼鳍的二阶鳍鱼</p></figcaption></figure>

如图所示，这个题就有两个鱼鳍。

不过，两个鱼鳍怎么去推理呢？把这两个鱼鳍当成整体就行了。不过如果初学的话，分开讨论一次也是必不可少的，主要是练习这种列举情况的思维，避免遗漏：

* 如果鱼鳍都不是正确填数
* 如果鱼鳍至少有一个是正确的填数

这两个就是鱼鳍的全部情况。

> 这你要是还不能理解那就上数学式子呗。“都不是”就等价于 $$=0$$ 个位置填；那么他的相反情况就只能是 $$>0$$ 个位置填，也就是说至少有一处。
>
> 严谨一点来讲，$$=0$$ 的相反情况是 $$\neq 0$$。不过，单元格数量不可能为负数，所以就不存在 $$<0$$ 的情况。

那么，直接假设一波：

* 如果鱼鳍都不是正确填数：
  * 此时鱼直接成立，按鱼的标准删数规则来删即可；
* 如果鱼鳍至少有一个是正确的填数：
  * 要么 `r5c8 = 1`，则删的是这个单元格所在行、列、宫的其他空格的候选数 1；
  * 要么 `r6c8 = 1`，则删的是这个单元格所在行、列、宫的其他空格的候选数 1。

所以这回就从两个情况变为了三个情况了。不过，因为鱼鳍都在同一个宫里，取了交集之后删数似乎并不受影响。因此，删数就跟其实 `r56c8(1)` 只有其中一处有鱼鳍的情况，所造成的删数其实是一样的。

所以，如果一个鱼用到了多个鱼鳍，那么只要鱼鳍能在同一个宫里，并且只要鱼鳍也都和鱼的其中某一个格子同宫（之前问题 1 里的结论），这样的鱼就是合理的。

另外，看起来似乎鱼鳍最多就只能两个。因为再多就容不下同一个宫了。实际上不是的。鱼鳍最多可以有 4 个。下面就是一则有 4 个鱼鳍的题目：

<figure><img src="../../.gitbook/assets/images_0049.png" alt="" width="375"><figcaption><p>有四个鱼鳍的三阶鳍鱼</p></figcaption></figure>

如图所示。这是一个三阶鳍鱼，并有四个鱼鳍 `r23c89(1)` 。

不过，推理我就省略了。这个的推理过程和前面的也都完全一样。

### 问题 3：有些别扭的鳍鱼 <a href="#a-weird-finned-fish" id="a-weird-finned-fish"></a>

我们再来看一则例子。

<figure><img src="../../.gitbook/assets/images_0053.png" alt="" width="375"><figcaption><p>这都啥玩意儿啊？</p></figcaption></figure>

如图所示。和前面的鱼鳍的推理方式完全一样地思考这个题，就会有点奇怪：

* 如果 `r1c2 = 5`，则所在的行、列、宫的其他空格都不能填入 5，删除它们；
* 如果 `r1c2 <> 5`，则……这应该叫什么，三阶鱼？它成立……等会，它真的成立吗？

问题肯定出现在第二点上。如果我们忽略掉 `r1c2(5)`，视它不存在的话，那么这个鱼用到的格子的分布是：

* `r1` 上有 `r1c89`；
* `r4` 上有 `r4c19`；
* `r8` 上有 `r8c89`。

这甚是奇怪。当鱼鳍不存在的时候，单看 `r18`，二阶鱼似乎已经成立，而根本轮不到三阶鱼什么事情。实际上也确实如此。那么这个删数看起来就会显得非常奇怪。这二阶鱼怎么可能删得到 `r23c1(5)`？

这个问题，你需要使用类似之前数组显隐性互补之后会出现大数组里包含小数组的那种思维去理解它。虽然二阶鱼已然成立，但是我们这里算上 `r4` 也未尝不可。大不了这个时候（鱼鳍不存在的时候），`r4c1` 就是 5 就行了。鱼的排列假设是看的行，然后发现它分布在几个不同的列里，然后发现列上必有一处填入。而当这个鱼成立时，`r4` 的两处，会使得 `r4` 真正意义上只有 `r4c1 = 5`。

之所以把这个结构给算上，还是因为鱼鳍的位置稍显特殊一些。我们刚才说，鱼鳍必须要在和鱼的某一个用到的单元格同一个宫。但这个题似乎并没有满足这一点。但是你仔细看看，它真的不满足吗？

是的。这个鱼本身就是残缺的。它缺失了一个重要的单元格，是 `r1c1(5)`。这个格子如果被我们补上，那么这个鱼的推理可能就不会那么难以理解。而这个鱼鳍和鱼的某一个格子同宫，在这个例子里是隐晦了一些：鱼鳍其实是跟不存在的“假肢”位于同一个宫里。

对于这种跟假肢位于同一个宫的鱼鳍，在鱼里面也是广泛存在的。这点和之前学到的数组互补之后会被小数组所代替稍微不同。数组那个可以被代替，而且是无条件代替掉，因为数组没有鱼鳍一说。

好了。至此我们就把鱼鳍的基本概念给各位说明白了。下一节我们将继续针对鱼鳍，给各位带来更加奇葩的用法。
