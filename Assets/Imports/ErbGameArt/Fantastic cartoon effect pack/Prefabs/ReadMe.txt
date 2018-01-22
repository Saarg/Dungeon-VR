After changing the prefab size, you must change the value of the pivot 1x1 (only when pivot > 1)!
It is in the render tab in the particle system.

Example: 

before
size = [1][1][1]
pivot = [0][30][0]


after
size = [3][3][3]
pivot = [0][90][0]

or
size = [1.5][1.5][1.5]
pivot = [0][45][0]

To change the transparency of the tornado material, use "Custom data tab - vector x" with "Custom vertex stream(z)" (Render tab) in particle system!

Don't forget to uncheck "loop" (main tab in a particle system) from all parts of the effect before use (if you need). 