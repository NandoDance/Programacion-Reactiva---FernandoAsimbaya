Los operadores usados en este proyecto son:

Select: transforma los eventos en strings descrptivos
Merge: unificar eventos creado, modificado y elminado
DistinctUntilChanged: emitir eventos que son diferentes al anterior
Timestamp: agregar marca de tiempo a cada evento
Buffer: agrupar eventos en intervalos de 5 segundos
