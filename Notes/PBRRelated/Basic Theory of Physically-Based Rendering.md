# Basic Theory of Physically-Based Rendering

[Source](https://marmoset.co/posts/basic-theory-of-physically-based-rendering/)

## Diffusion & Reflection

Diffusion and reflection - also known as "diffuse" and "specular" light respectively - are two terms describing the most basic separation of surface/light interactions . Most perple will be familiar with these ideas on a practical level , but may not know how they are physically distinct .

When light hits a surface boundary some of it will reflect - that is , bounce off - from the surface and leave heading in a direction on the opposing side of the surface normal . This behaviour is very sililar to a ball thrown against the ground or a wall - it will bounce off at the opposite angle . On a smooth surface this will result in a mirror-like appearance . The word "specular" , often used to describe the effect , is derived from the latin for "mirror" (it seems "specularity" sounds less awkward than "mirrorness").

Not all light reflects from a surface , however . Usually some will penetrate into the interior of the illuminated object . There it will either be absorbed by the material (usually converting to heat) or scattered internally . Some of this scattered light may make its way back out of the surface , then becoming visible once more to eyeballs and cameras . This is known by many names : "Diffuse Light" , "Diffusion" , "Subsurface Scattering" - all describe the same effect .

![Diffusion](./Asset/00Diffusion.jpg)

The absorption and scattering of diffuse light are often quite different for different wavelengths of light , which is what gives objects their color (e.g. if an object absorbs most light but scatters blue , it will appear blue) . The scattering is often so uniformly chaotic that it can be said to appear the same from all directions - quite different from the case of a mirror ! A shader using this approximation really just needs one input : "albedo" , a color which describes the fractions of various colors of light that will scatter back out of a surface . "Diffuse Color" is a phrase sometimes used synonymously .

## Translucency & Transparency

In some cases diffusion is more complicated - in materials that have wider scattering distances for example , like skin or wax . In these cases a simple color will usually not do , and the shading system must take into account the shape and thickness of the object being lit . If they are thin enough , such objects often see light scattering out the back side and can then be called translucent . If the diffusion is even lower yet (in for example , glass) then almost no scattering is evident at all and entire images can pass through an object from one side to another intact . These behaviours are different enough from the typical "close to the surface" diffusion that unique shaders are usually needed to simulate them .

## Energy Conservation

With these descriptions we now have enough information to draw an important conclusion , which is that **reflection** and **diffusion** are mutually exclusive . This is because , in order for light to be diffused , light must first penetrate the surface (that is , fail to reflect) . This is known in shading parlance as an example of "energy conservation" , which just means that the light leaving a surface is never any brighter than which fell upon it originally .

This is easy to enforce in a shading system : one simply subtracts reflected light before allowing the diffuse shading to occur . This means highly reflective objects will show little to no diffuse light , simply because little to no light penetrates the surface , having been mostly reflected . The converse is also true : if an object has bright diffusion , it cannot be especially reflective .

![EnergyConservation](./Asset/01EnergyConservation.jpg)

Energy conservation of this sort is an important aspect of physically-based shading . It allows the artist to work with reflectivity and albedo values for a material without accidentally violating the laws of physics (which tends to look bad) . While enforcing these constraints in code isn't strictly necessary to producing good looking art , it does serve a useful role as a kind of "nanny physicist" that will prevent artwork from bending the rules too far or becoming inconsistent under different lighting conditions .

## Metals

