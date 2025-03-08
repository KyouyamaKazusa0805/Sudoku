---
description: Sashimi Fish
---

# 孪生鱼

前面我们学到了鱼鳍的基本用法，是用来假设鱼鳍自身的填入和不填的两种状态，从而影响鱼的推断，得到删数的一种特殊用法。

但是，有些时候，鱼鳍的存在可以使得鱼本身会具有一些特殊的特征，例如一连存在两个鱼，只有鱼鳍不一样的特殊情况。

## 二阶孪生鱼（Siamese X-Wing） <a href="#siamese-x-wing" id="siamese-x-wing"></a>

<figure><img src="../../.gitbook/assets/image (2) (1) (1) (1) (1) (1).png" alt="" width="375"><figcaption><p>二阶孪生鱼</p></figcaption></figure>

如图所示。你肯定在之前的文章里发现过，一些题的二阶退化鱼是比较特殊的，就像是图上这样，它其实是两个鱼同时长在了一起：

<figure><img src="../../.gitbook/assets/image (1) (1) (1) (1) (1) (1) (1).png" alt=""><figcaption><p>拆开看的两条二阶退化鱼</p></figcaption></figure>

可以看到，左图和右图各是一个二阶退化鱼，只是它们的鱼鳍不同。不同的点是，其中一个鱼的鱼鳍，在另一边变为了鱼的一部分；而一边的鱼的一部分在另一边就改成了鱼鳍：它俩互换了。

这个神奇的现象在鱼里也是广泛存在的。我们就把两个鱼绝大部分用相同的单元格，而只有一小部分会和鱼鳍互换的这种特殊鱼的状态称为**孪生鱼**（Siamese Fish）。

> 孪生一词来源于暹罗猫的暹罗（是一个地名）。相传几百年前这个地方诞生双胞胎的概率比其他地方都大，因此出名了。现如今，暹罗也会时不时用作术语，表示这种孪生的状态。例如计算机里的 siamese network（孪生网络）。

所以，孪生鱼其实并不算是一种数独技巧。它更像是归并了两条只有鱼鳍不同的鱼，并且鱼鳍会变为另一边的鱼的一部分，这样的一种特殊状态。

下面我们再来看一些例子。

## 三阶孪生鱼（Siamese Swordfish） <a href="#siamese-swordfish" id="siamese-swordfish"></a>

<figure><img src="../../.gitbook/assets/image (2) (1) (1) (1) (1) (1) (1).png" alt="" width="375"><figcaption><p>三阶孪生鱼</p></figcaption></figure>

如图所示。它是如下两条鱼的合并。

<figure><img src="../../.gitbook/assets/image (3) (1) (1) (1) (1) (1).png" alt=""><figcaption><p>拆开看的两条三阶退化鱼</p></figcaption></figure>

## 四阶孪生鱼（Siamese Jellyfish） <a href="#siamese-jellyfish" id="siamese-jellyfish"></a>

<figure><img src="../../.gitbook/assets/image (4) (1) (1) (1) (1) (1).png" alt="" width="375"><figcaption><p>四阶孪生鱼</p></figcaption></figure>

如图所示。它是如下两条鱼的合并。

<figure><img src="../../.gitbook/assets/image (5) (1) (1) (1) (1).png" alt=""><figcaption><p>拆开看的两条四阶退化鱼</p></figcaption></figure>

## 孪生鱼的类型 <a href="#types-of-siamese-fish" id="types-of-siamese-fish"></a>

在上面这一个例子里，我们发现到它和前面两则例子里的不同之处。当它拆分开之后，左图的这个是一个四阶退化鱼，而右图这个是一个四阶鳍鱼（因为 `r4` 里鱼鳍扣除后，还有两处可以填 9 的位置）。

所以，孪生鱼在定义上并未严格对是否是退化鱼，或者是否是鳍鱼有限制。实际上，孪生鱼会有三种归并的状态：

* 两个鳍鱼
* 两个退化鱼
* 一个鳍鱼和一个退化鱼

你能猜到它们的名字吗？答案公布：

* **孪生鳍鱼**（Siamese Finned Fish）：两个鳍鱼
* **孪生退化鱼**（Siamese Sashimi Fish）：两个退化鱼 或 一个鳍鱼和一个退化鱼

前面的三个例子，前两个是孪生退化鱼，最后一个也是，不过它是鳍鱼和退化鱼的合并。下面我们来看一个孪生鳍鱼的例子。

<figure><img src="../../.gitbook/assets/image (7) (1) (1) (1).png" alt="" width="375"><figcaption><p>四阶孪生鳍鱼</p></figcaption></figure>

这个例子我就不展开成两个了。我相信你看过了前面三个例子后，这个例子应该看得懂。

再来看一个孪生退化鱼。不过这个有三个鱼鳍。

<figure><img src="../../.gitbook/assets/image (8) (1) (1) (1).png" alt="" width="375"><figcaption><p>三个鱼鳍的孪生退化鱼</p></figcaption></figure>

最后看一个有四个鱼鳍的四阶孪生退化鱼，也是鳍鱼和退化鱼合并的。

<figure><img src="../../.gitbook/assets/image (6) (1) (1) (1).png" alt="" width="375"><figcaption><p>四阶孪生退化鱼</p></figcaption></figure>
