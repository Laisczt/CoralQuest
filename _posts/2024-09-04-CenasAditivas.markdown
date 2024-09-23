---
layout: post
title:  "Cenas Aditivas"
date:   2024-09-04 12:00:00 -0300
#categories: jekyll update
---

A UI é uma parte muito importante de qualquer jogo, mais importante ainda é que ela seja consistente entre fases diferentes. A forma que vimos para criar UIs até agora não é ideal, pois ao fazer alterações, precisamos replicá-las em todas as cenas que a usam.

Uma forma mais versátil de fazer nossa UI é colocá-la numa cena própria, e então carregar tal cena de forma aditiva(ou, por cima) da cena da fase.

{% highlight c# %}
void Start()
{
	StartCoroutine(LoadUI());
}
IEnumerator LoadUI()
{
	var _UIScene = SceneManager.LoadScene("UI", new LoadSceneParameters(LoadSceneMode.Additive));   // Carrega a UI por cima da cena inicial

        while(!_UIScene.isLoaded)   // Espera até a UI estar carregada
        {
            yield return null;
        }
}

{% endhighlight %}

Note que carregar cenas aditivamente pode demorar um pouco, por isso é recomendado usar uma [Corotina](../../../2024/09/05/Corotinas.html)

A desvantagem desse método é que referências entre objetos da UI e da fase devem ser tratadas com mais atenção. Utilizar o [Padrão Singleton](../../../2024/09/03/Singleton.html), e tags pode ajudar bastante. Outra forma é utilizar a referência a cena retornada pela função LoadScene().

{% highlight c# %}
_UIScene.GetRootGameObjects()[0].transform.Find("Health Bar").SetFullHP();
{% endhighlight %}


