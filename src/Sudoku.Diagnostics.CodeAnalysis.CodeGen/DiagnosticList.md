| Diagnostic ID | Category     | Severity | Title                                                        | Description                                                  |
| ------------- | ------------ | -------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| SCA0101       | Sunnie.Usage | Error    | The type parameter lacks a CRTP-constraint clause            | The type parameter lacks a CRTP-constraint clause; you should append the type constraint '{0}' into the whole clause 'where {1} : {0}' |
| SCA0102       | Sunnie.Usage | Error    | The type parameter lacks a CRTP type constraint              | The type parameter lacks a CRTP type constraint; you should apply the constraint like: 'where {0}: {1}' |
| SCA0201       | Sunnie.Usage | Error    | The discarded parameter can't be used or referenced unless a 'nameof' expression |                                                              |
| SCA0202       | Sunnie.Usage | Error    | Discard parameter can't be 'out', 'ref' or 'params' one      |                                                              |
| SCA0203       | Sunnie.Usage | Error    | Can't apply 'IsDiscardAttribute' onto a parameter that has already discarded |                                                              |

