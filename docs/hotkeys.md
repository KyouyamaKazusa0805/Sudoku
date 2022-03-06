# 快捷键

程序使用了一些快捷键，可以在全局或局部页面使用。下面列举一下。

##  题目分析页面（对应 `SudokuPage` 类型）

* 命令栏
  * 编辑题目功能列表
    * <kbd>Control + O</kbd>：从本地路径里选取一个文件打开导入到程序之中；
    * <kbd>Control + S</kbd>：保存当前程序的题目到本地；
    * <kbd>Control + C</kbd>：复制题目的代码；
    * <kbd>Control + V</kbd>：将剪切板里的代码粘贴到程序里；
    * <kbd>Control + Tab</kbd>：固定题目里的所有填入数字；
    * <kbd>Control + Shift + Tab</kbd>：解除固定，将全部提示数转为可修改的填入数字；
    * <kbd>Control + Z</kbd>：撤销一个步骤；
    * <kbd>Control + Y</kbd>：还原上一个撤销的步骤。
  * 出题和分析题目
    * <kbd>Control + H</kbd>：生成一个题目。
* 数独盘面
  * <kbd>数字 0</kbd>：删除当前单元格的填数；
  * <kbd>数字 1-9</kbd>：往当前鼠标指向的单元格里填入当前按键对应的数值；
  * <kbd>Shift + 数字 1-9</kbd>：删除当前单元格里的对应数值的候选数。

其它页面的功能要么还没做，要么不需要快捷键，所以敬请期待。