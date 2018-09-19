Posibles Comandos a enviar desde la consola
-------------------------------------------

all:[mensaje]
	Envia el mensaje a todos los clientes conectados

pares:[mensaje]
	Envia el mensaje a todos los clientes que pertenecen al grupo "pares"

impares:[mensaje]
	Envia el mensaje a todos los clientes que pertenecen al grupo "impares"

[index]:[mensaje]
	Envia el mensaje a un cliente en particular. Index debe ser un valor numerico, y reprenta el index de la lista de clientes conectados (comienza en uno)


NOTA:
	1-Cada vez que se conecta un cliente, se agrega al grupo "pares" o "impares"
	2-El index comienza en 1