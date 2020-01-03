# HLHML
Un interpreteur pour des scripts écrits en français (High Level Humain Machaine Language)

## Exemple de script
```
a vaut 5.
b vaut 7.

Afficher a " + " b " = ? ".

Lire la reponse.

Si la reponse est égal à a + b, afficher "Bonne réponse!" sinon afficher "Mauvaise réponse".
```

## Sortie
```
5 + 7 = ? 
```
L'utilisateur tape sa réponse
```
5 + 7 = ? 12
```
Le programme affiche
```
5 + 7 = ? 12
Bonne réponse!
```

## Lexique
<a href="https://github.com/freddycoder/HLHML/wiki/Lexique">Visiter la documentation</a>

## Générer l'image de l'arbre syntaxique
Selon le script suivant : 
```
i vaut 0.
Tant que i est plus petit que 3,
i = i + 1.
Si i est égal à 3, afficher i " ".
Ensuite, afficher i
```
En appelant l'executable en ligne de commande :
```
HLHML.exe monScript.fr -t imageDestination.bmp
```
Le programme va produire l'image suvante
![impossible de trouver l'image...](https://raw.githubusercontent.com/freddycoder/HLHML/master/exempleAST.bmp)
</br>
Executer le même script en ligne de commande
```
HLHML.exe monScript.fr
```
La sortie sera
```
3 3
```
