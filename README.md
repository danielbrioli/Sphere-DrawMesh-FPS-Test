# Sphere-DrawMesh-FPS-Test

This is a simple sphere generator that creates a sphere in two ways:

The first is with individual GameObjects, each representing a triangle, holding its mesh, etc.

The second is with the Graphics.DrawMesh() method.

The intent is to display the stark differences between the two methods, along with my favorite way of creating a mesh procedurally.

The GameObject method gives complete control in Unity over the sphere, since each triangle the sphere is made from is an individual object with components.

The Graphics.DrawMesh() method gives less control outside of scripting, but the FPS increase is massive. It also only is affected by ambient lighting, since the Graphics.DrawMesh() method ignores real time lighting.

This is (in general) my preferred sphere generation method. It creates a sphere with equal distances between all points by creating triangles and continually subdividing these triangles based on a resolution parameter. Other methods of sphere generation can cause their own unique issues. For instance, a latitude/longitude-style sphere has lower resolution at its "equator" and higher resolution at its "poles". A cube-based method (where you create a cube out of triangles and then normalize the Vector3s) has the downfall of oddly-spaced triangles across its surface due to the normalization of the cube.
