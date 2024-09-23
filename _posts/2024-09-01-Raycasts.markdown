---
layout: post
title:  "Raycasts"
date:   2024-09-01 12:00:00 -0300
#categories: jekyll update
---

Um raycast é, reduzidamente, uma linha traçada entre um ponto de origem e um de um ponto destino, a utilidade disto vem de que podemos detectar com quais objetos essa linha colidiu em seu trajeto.

Exemplos de uso de Raycast:
- Determinar se um inimigo consegue ver o jogador quando há obstáculos no caminho
- Saber se o jogador está olhando para um objeto interajível
- Simular tiros de armas quando não queremos usar projéteis
- Calcular o quão longe o jogador está do chão

O último pode ser feito da seguinte forma:

{% highlight c# %}

LayerMask chaoMask;
float distanciaChao;

void Start()
{
	chaoMask = LayerMask.GetMask("Chao");
	// Define uma máscara para a camada chão
}

void Update()
{
	var hit = Physics2D.Raycast(transform.position, Vector2.down, 100.0f, chaoMask)
	// Faz um Raycast da posição do jogador para baixo, com uma distância máxima de 100 unidades, que só pode colidir com objetos na camada Chao
	// a variável hit guarda as informações da primeira colisão desse raycast
	
	if(hit != null) // se o raycast recordou uma colisão, pegamos sua distância
	{
		distanciaChao = hit.distance;
	}
	else
	{
		distanciaChao = -1;
	}
}

{% endhighlight %}

