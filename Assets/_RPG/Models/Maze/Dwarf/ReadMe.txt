Normalement, tout devrait être assez instinctif, mais dans le doute:

Change juste les deux fichiers de texture que j'ai mis là dans les matériaux du projet que t'as déjà, c'est pour que le sol et le plafond aient des textures qui marchent bien (j'ai enlevé les traits, vu qu'on peut pas savoir si ça tourne ou si c'est droit).

Tout sauf les piliers est reglé pour que les placer aux mêmes coordonées les assemblent correctement.

Je sais pas trop comment tu vas t'y prendre pour les piliers, honnêtement. Au pire, demande moi et je déplace le point de pivot.

En parlant de piliers, Pillar_straight est assez logique, il faudra juste que tu l'inverse une fois sur deux. Pillar_ClosedTurn est en un seul morceau et (si je me souviens bien) a son point de pivot au centre du sol, donc rien à faire avec celui là. Pillar_OpenTurn, par contre, se relie entre deux Pillar_straight.

Horiz_Bar_top est le truc avec le gros rubis entre les pilliers. Toi qui vois si tu veux y placer.

Et pour les props, tu les as dans la scène de base.

Donc voilà, normalement t'as tout. Bon boulot =P 