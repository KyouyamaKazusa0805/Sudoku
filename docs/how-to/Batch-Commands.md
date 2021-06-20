这里罗列一些批处理的指令。



| 命令格式                                                     | 意义                                     |
| ------------------------------------------------------------ | ---------------------------------------- |
| `create workspace with width <width> height <height>`        | 创建一个指定宽度和高度的画板。           |
| `fill given <digit> in <cell>`                               | 为题目上的指定格子上填入一个提示数字。   |
| `fill modifiable <digit> in <cell>`                          | 为题目上的指定格子上填入一个可修改的数。 |
| `fill candidate <digit> in <cell>`                           | 为题目的指定位置上填入一个候选数。       |
| `draw cell <cell> with color a <a> r <r> g <g> b <b>`        | 给指定的单元格涂上一个颜色。             |
| `draw candidate <candidate> with color a <a> r <r> g <g> b <b>` | 给指定的候选数涂上一个颜色。             |
| `draw region <region> with color a <a> r <r> g <g> b <b>`    | 给指定的区域涂上一个颜色。               |
| `draw row <row> with color a <a> r <r> g <g> b <b>`          | 给指定的行涂上一个颜色。                 |
| `draw column <column> with color a <a> r <r> g <g> b <b>`    | 给指定的列涂上一个颜色。                 |
| `draw block <block> with color a <a> r <r> g <g> b <b>`      | 给指定的宫涂上一个颜色。                 |
| `draw chain from <candidate1> to <candidate2> type (line\|strong\|weak\|chain)` | 画一条链。                               |
| `draw cross <cell>`                                          | 画一个叉叉到一个格子上。                 |
| `draw circle <cell>`                                         | 画一个圆圈到一个格子上。                 |
| `save to <path>`                                             | 将绘制的图片保存到指定的路径上。         |
| `close`                                                      | 退出绘图，删除相关的资源。               |

