---
layout: post
title:  "Multiplas Câmeras"
date:   2024-09-07 12:00:00 -0300
#categories: jekyll update
---

Existem várias formas de utilizar várias câmeras simultaneamente no Unity.

Coral Quest utiliza uma câmera principal que vê o nível, e uma câmera de background que somente vê o plano de fundo.

Para alcançar esse efeito, posicione sua tela de fundo em algum lugar em sua cena com uma câmera apontando para ela (garanta que a câmera não possua a tag 'Main Camera')

![Camera e tela de fundo em cena](../../../img/telacam.png)

Em seguida, crie um Render Texture na aba de projeto, e o configure com as dimensões desejadas

![Propriedades Render Texture](../../../img/rendertexture.png)

Passe a render texture criada como textura de output da camera

![Atribuição output texture](../../../img/outputtexture.png)

Crie um canvas como objeto filho da câmera principal, e defina o modo de renderização como Screen Space - Camera

![Canvas filho](../../../img/cameracanvas.png)

Crie uma Raw Image como filha do canvas em UI > Raw Image e utilize a render texture como textura

![Textura no Canvas](../../../img/texturecanvas.png)

Por fim, defina a ancoração da imagem para stretch em ambas as direções, segurando alt para copiar a posição

![Anchor Presets](../../../img/anchorpresets.png)

Veja também outras formas de utilizar múltiplas câmeras:

[https://www.youtube.com/watch?v=4A-ptevH6-w](https://www.youtube.com/watch?v=4A-ptevH6-w)

[https://www.youtube.com/watch?v=bbnVpPiQ_rU](https://www.youtube.com/watch?v=bbnVpPiQ_rU)


