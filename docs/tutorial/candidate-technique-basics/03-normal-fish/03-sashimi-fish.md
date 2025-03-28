﻿---
description: Sashimi Fish
---

# 退化鱼

在鳍鱼的最后，我们列出了一种情况，就是大鱼里有小鱼的情况。鱼鳍的存在，才会让这样的结构能够稳定存在。

下面我们来看另外一种鱼鳍存在，鱼才会成立的情况。

## 二阶退化鱼（Sashimi X-Wing） <a href="#sashimi-x-wing" id="sashimi-x-wing"></a>

<figure><img src="../../.gitbook/assets/images_0231.png" alt="" width="375"><figcaption><p>二阶退化鱼</p></figcaption></figure>

如图所示。我们先看这个鱼，显然这个鱼长相就非常奇特——它就用了三个单元格。

如果我们补上 `r6c9(8)`，这个鱼就是一个很普通的二阶鳍鱼。但是看起来似乎这个删数跟它有没有，没有任何关系。

当鱼鳍不存在的时候，鱼只有三个单元格。我们只看的是 `r69` 里数字 8 的填数情况，而这次，`r6` 只有一个单元格可填了，所以 `r6` 直接会把数字填到 `r6c2` 上，然后 `r9c9` 因为 `r6c2` 填入之后直接也能出数。

于是，整个鱼由于鱼鳍的缺失而直接散架。但是，我们不难发现的是，由于 `r6c2` 和 `r9c9` 的填数，一个在 `c2` 一个在 `c9`。所以这点和鱼的性质完全一样：仍然能保证两列都会出现 8 的填数。

所以，这种鱼的情况从之前的排列直接简化成了如下两个情况：

* 如果 `r6c7 = 8`，则删除 `r6c7` 所在行列宫其他位置上的候选数 8；
* 如果 `r6c7 <> 8`，则鱼直接瓦解，可直接得到 `r6c2` 和 `r9c9` 同时填 8，仍然可保证 `c29` 的鱼的规则删除候选数 8。

于是，`r5c9 <> 8` 仍然可以得到。

这个结构我们称为**二阶退化鱼**或**退化二阶鱼**（Sashimi X-Wing）。早期也叫**退化二链列**。

我知道你肯定没怎么看明白。一来是结构非常不优雅，二来是云里雾里的内容很难知道这种结构为什么就成立了。这个我们先放一边。我先把所有例子都给你讲完，然后再来说为什么这样的结构始终成立。

哦对，我再给你看一则例子。这个例子可以自己看一下。鱼鳍有两个就意味着我们直接合并讨论就行。其他的逻辑和前面叙述的完全一样。

<figure><img src="../../.gitbook/assets/images_0006.png" alt="" width="375"><figcaption><p>两个鱼鳍的二阶退化鱼</p></figcaption></figure>

## 三阶退化鱼（Sashimi Swordfish） <a href="#sashimi-swordfish" id="sashimi-swordfish"></a>

<figure><img src="../../.gitbook/assets/images_0077.png" alt="" width="375"><figcaption><p>三阶退化鱼</p></figcaption></figure>

如图所示，我们仍旧使用鱼鳍的视角讨论它的两种状态：

* 如果 `r3c7 = 7`，则删除所在行列宫其他位置的候选数 7；
* 如果 `r3c7 <> 7`，则剩余的 5 个鱼使用的单元格会直接因为 `r3` 而瓦解，并顺次得到 `r3c3`、`r5c4` 和 `r9c9` 填 7。与此同时，也保证了 `c349` 各列都出现一个 7，于是鱼的删数规则仍然成立，删除 `c349` 其余位置的候选数 7。

然后我们按交集的规则，仍然可以得到 `r2c9 <> 7` 的结论。

我们把这个称为**三阶退化鱼**或**退化三阶鱼**（Sashimi Swordfish）。早期也叫**退化三链列**。

下面我们再来看一则例子。

<figure><img src="../../.gitbook/assets/images_0106.png" alt="" width="375"><figcaption><p>三阶退化鱼，另外一个例子</p></figcaption></figure>

如图所示。这个例子我得说一下，因为有点难理解。

和前面假设的方式一样，不过这一次我们要把两个鱼鳍当成整体进行讨论：

* 如果鱼鳍都不是正确填数，则剩下的 5 个单元格由于 `r6` 只剩下一个位置可填，导致结构瓦解，并依次得到 `r6c6`、`r4c1` 和 `r3c7` 分别填上 4 的结果。并因为这种填法，`c167` 都会出现一个 4，所以 `c167` 三列里的其他单元格的候选数 4 均可删除；
* 如果鱼鳍里至少有一个是正确的填数，则：
  * 如果是 `r4c9 = 4`，则删除 `r4c9` 所在的行列宫其余位置的候选数 4；
  * 如果是 `r6c9 = 4`，则删除 `r6c9` 所在的行列宫其余位置的候选数 4。

当我们按照这个规则讨论之后，一共可以列举出三种填法，而三种填法的交集是 `r5c7(4)`。因此，`r5c7 <> 4` 是这个题的结论。

## 四阶退化鱼（Sashimi Jellyfish） <a href="#sashimi-jellyfish" id="sashimi-jellyfish"></a>

<figure><img src="../../.gitbook/assets/images_0130.png" alt="" width="375"><figcaption><p>四阶退化鱼</p></figcaption></figure>

如图所示，这个例子也是如此讨论。讨论鱼鳍的两种情况，这一点和前面一样，因此这个例子就不解释了，只是规格稍微大一些。

这个我们叫做**四阶退化鱼**或**退化四阶鱼**（Sashimi Jellyfish），早期也叫**退化四链列**。

## 说说为什么这种结构能奏效 <a href="#questions-about-why-sashimi-fishes-are-valid" id="questions-about-why-sashimi-fishes-are-valid"></a>

我们把这种结构称为**退化鱼**（Sashimi Fish）。所谓退化鱼的本质逻辑就是去掉鱼鳍之后，某个行列会直接瓦解成为单一的填数（行列排除），进而牵一发动全身。

### 问题 1：为什么鱼直接瓦解仍然能让鱼能具有合理的删数？ <a href="#question-1" id="question-1"></a>

先来说第一个问题。这个问题解释起来比较简单。

要说这个问题，我们需要回忆一下鱼的推理过程。鱼的推理过程是为了保证一个 $$n$$ 行（列）的某一个候选数刚好只出现在 $$n$$ 个不同的列（行）里。由于我们为了保证数字在排列的时候能够规避产生重复的问题，而行列的数量一致，那么我们只好每一个列（行）都分配一个这个数字的填写，这样互相不冲突。而正是因为这样的摆放是唯一可以规避重复的方式，而行列数量一致，导致了它不会有列（行）里完全不出现这个数字的问题。所以，所有这些列（行）的其他单元格都没有可能填入这个数。

而这种鱼的类型呢？不管它缺不缺单元格，你就说满不满足吧。你可以把结构代入这个说法，你可以发现它仍然是满足的。尤其是最开始这一句。它缺格子又不是多格子，所以缺少的格子并不会导致分布的行列超出去。这是它的本质原因。

所以，大大方方用就行了。你是第一次了解这一点，所以看起来略显别扭。但是实际上并不影响。

### 问题 2：有些单元格似乎在推理的过程之中就压根没用过 <a href="#question-2" id="question-2"></a>

<figure><img src="../../.gitbook/assets/images_0231.png" alt="" width="375"><figcaption><p>还是刚才的二阶退化鱼</p></figcaption></figure>

这个问题需要解释一下。如图所示，这还是刚才的那个二阶鱼的例子。

在完整的推理过程之中，要么鱼鳍 `r6c7` 填 8，要么就是 `r6c2` 和 `r9c9` 填 8。整个推理过程自始至终都没用到左下角的 `r9c2(8)`。

这不免会让人引起疑心：这个 `r9c2(8)` 怕不是个充数的。它存在的意义是什么？我们可否因为它用不上而删除它？

这个问题的答案肯定是否定的。它有用，而且不能删除。可它用在哪里了呢？这里就要说到退化鱼一个不太优雅的点了：它需要内部列举一下填数才能知道它存在的意义。

我们倘使 `r9c2 = 8` 看看到底会出什么效果。我们知道，退化鱼有一个特征：鱼鳍所在的这个行列上，去掉鱼鳍会因为只剩下一个位置而直接造成瓦解（那肯定得是鱼鳍所在的这个行列上，因为不在鱼鳍的行列上只剩下一个位置不就直接成行列排除了么）。而这个退化鱼的特征我们现在要用上去了。因为 `r9c2 = 8`，所以对于可导致瓦解的 `r6` 而言，此时鱼鳍这个位置会被迫填入 8。是的，你没有用到的这个位置，它其实是鱼鳍填入的情况，才会遇到的。而这一点，它并未体现在我们前文的推理过程之中，因为它不重要。

<figure><img src="../../.gitbook/assets/images_0077.png" alt="" width="375"><figcaption><p>还是刚才的三阶退化鱼的例子</p></figcaption></figure>

再来看这个例子。这个例子里，要假设填出数字的位置，然后还有鱼鳍，实际上还有两处单元格没用到，即这里的 `r5c3` 和 `r9c4`。

实际上你假设其中任意一个填入，最终都会落到鱼鳍 `r3c8 = 7` 的填法结果上。

所以，这个问题的结果已经显而易见了：没用到的位置其实是和鱼鳍绑定起来的同一个填数情况。

### 问题 3：似乎鱼鳍仍然没有跟鱼的本体的某一个格处于同一个宫 <a href="#question-3" id="question-3"></a>

这个前面解释过了。我还是说明一下。退化鱼的特征是，去掉鱼鳍后，鱼的某一个行列上会直接只剩下一处单元格可填，进而造成鱼的结构直接全盘瓦解。所以很明显我们知道的是，退化鱼本质是残缺鱼的一种特殊状态。

既然是残缺的，就有可能会出现和鱼鳍同宫的那个（或那些）位置残缺的状况。此时你需要把它给补回去才能勉强看明白。这是这种残缺鱼不太能让人理解的一个地方。

不过，经过前面细致的说明之后，我们不难发现，这种补充是随时可用的。因为残缺鱼本身不影响鱼形成的推理过程，所以你完全可以补回去让他看起来更好看点，理解也更好理解一些，所以在看这种鱼的时候建议你补一下空格，然后再去理解。

## 退化鱼的精确定义 <a href="#rigor-definition-of-sashimi-fish" id="rigor-definition-of-sashimi-fish"></a>

终于来到退化鱼的最后一个需要说明的知识点了。在之前鳍鱼的那一节，最后我们留了一个题，给大家介绍了一种大鱼包含小鱼的这种特殊状态的鳍鱼，而似乎它看起来有些类似退化鱼的思维。

现在再给各位看一个例子。这个例子是退化鱼，但是它不能做到结构完整的瓦解：

<figure><img src="../../.gitbook/assets/images_0154.png" alt="" width="375"><figcaption><p>退化鱼，但不是整个鱼完全瓦解</p></figcaption></figure>

我们可以看到，假设鱼鳍 `r9c3 <> 8` 的时候，我们虽然可以得到 `r9c9 = 8` 的结果，但是得到之后我们通过排除只能得到 `r16c9 <> 8` 的结论。看起来似乎余下的部分并没有瓦解。

有人说 `b6` 里的 8 只有 `r4c8` 可以填了。这是不对的。因为它只是这个题满足的巧合。实际上在其他的题里，没用到的 `r5c789` 里可能会有 8 的存在，此时排除掉之后 8 仍然得不到出数。所以你只能把它当成巧合。

那么，这不能瓦解怎么又叫退化鱼了呢？

我还是把前一节的那个题拿过来给大家作对比。

<figure><img src="../../.gitbook/assets/images_0053.png" alt="" width="375"><figcaption><p>之前那个鳍鱼的例子</p></figcaption></figure>

对比了之后我们不难发现，这两个题还是有本质区别的：鳍鱼的这个例子，鱼鳍去掉后，`r4c1 = 5` 的结论是后出的。换言之，鱼鳍去掉后，鱼鳍的所在行（即这里的 `r1`）并没有受到影响，它还是两个候选数的分布。

但是前面的那个退化鱼的例子里，鱼鳍去掉后，鱼鳍的所在行只剩下了 `r9c9` 这一处可以填。

所以，退化鱼的精确的定义是：**鱼鳍在去掉后，鱼鳍的所在行列上（假设的那个行列）只能有一处可放的位置。如果满足这一点的带有鱼鳍的鱼，我们把它叫退化鱼；反之，它就只能当成鳍鱼来看待**。
