using System.IO;
using System.Reflection;
using Sudoku.XmlDocs;

string executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

//                   src\<ProjectName>\bin\debug\net5.0
//                                    |
//                                    |         src\<ProjectName>\bin\debug
//                                    |                       |
//                                    |                       |  src\<ProjectName>\bin
//                                    |                       |       |
//                                    |                       |       |  src\<ProjectName>
//                                    |                       |       |       |
//                                    |                       |       |       |      src
//                /--------------------------------------\ /----\  /----\  /----\  /----\
string rootPath = new DirectoryInfo(executingAssemblyPath).Parent!.Parent!.Parent!.Parent!.FullName;

await new DocumentationCommentOutputService(rootPath).ExecuteAsync();