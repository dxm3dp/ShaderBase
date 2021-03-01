# Bloom

[](https://catlikecoding.com/unity/tutorials/advanced-rendering/bloom/)

The amount of light that a display can produce is limited. It can go from black to full brightness, which in shaders correspond to RGB values 0 and 1. This is known as the low dynamic range - LDR - for light. How bright a fully white pixel is varies per display and can be adjusted by the used, but it's never going to be blinding.

Real life isn't limited to LDR light. There isn't a maximum brightness. The more photons arrive at the same time, the brighter something appears, until it becomes painful to look at or even blinding. Directly looking at the sun will damage your eyes.

To represent very bright colors, we can go beyond LDR into the high dynamic range - HDR. This simple means that we don't enforce a maximum of 1. Shaders have no trouble working with HDR colors, as long as the input and output formats can store values greater than 1. However, displays cannot go beyond their maximum brightness, so the final color is still clamped to LDR.

To make HDR colors visible, they have to be mapped to LDR, which is known as **tonemapping**. This boils down to nonlinearly darkening the image, so it becomes possible to distinguish between originally HDR colors. This is somewhat analogous to how our eyes adapt to deal with bright scenes, although tonemapping is constant. There's also the auto-exposure technique, which adjust the image brightness dynamically. Both can be used together. But our eyes aren't always able to do adapt sufficiently. Some scens are simply too bright, which makes it harder for us to see. How could we show this effect, while limited to LDR displays?

Bloom is an effect which messes up an image by making a pixels' color bleed into adjacent pixels. It's like blurring an image, but based on brightness. This way, we could communicate overbright colors via blurring. It's somewhat similar to how light can diffuse inside our eyes, which can become noticeable in case of high brightness, but it's mostly a nonrealistic effect.

## Blurring

The bloom effect is created by taking the original image, blurring it somehow, then combining the result with the original image. So to create bloom, we must first be able to blur an image.

### Downsampling

**Blurring an image is done by averaging pixels**. For each pixel, we have to decide on a bunch of nearby pixels to combine. Which pixels are included defines the filter kernel used for the effect. A little blurring can be done by averaging only a few pixels, which means a small kernel. A lot of blurring would require a large kernel, combining many pixels.

The more pixels there are in the kernel, the more times we have to sample the input texture. As this is per pixel, a large kernel can require a huge amount of sampling work. So let's keep it as simple as possible.

The simplest and quickest way to average pixels is to take advantage of the bilinear filtering built into the GPU. If we halve the resolution of the temporaty texture, then we end up with one pixel for each group of four source pixels. The lower-resolution pixel will be sampled exactly in between the original four, so we end up with their average. We don't even have to use a custom shader for that.

Using a half-size intermediate texture means that we're downsampling the source texure to half resolution. After that step, we go from the temporary to the destination texture, thus upsampling again to the original resolution.

This is a two-step blurring process where each pixel gets mixed up with the 4x4 pixel block surrounding it, in four possible configurations.

The result is an image that's blockier and a little blurrier that the original.

### Progressive Downsampling

Unfortunately, directly downsampling to a low resolution leads to poor result. We mostly end up discarding pixels, keeping only the averages of isolated groups of four pixels.

A better approach is to downsample multiple times, halving the resolution each step until the desired level is reached. That way all pixels end up contributing to the end result. 

### Progressive Upsampling

While progressive downsampling is an improvement, the result still gets too blocky too fast. Let's see whether it helps if we progressively upsample as well.

Iterating in both directions means that we end up rendering to every size twice, except for the smallest. instead of releasing and then claiming the same textures twice per render, let's keep track of them in an array. We can simple use an array field fixed at size 16 for that, which should be more than enough.

### Custom Shading

To improve out blurring, we have to switch to a more advanced filter kernel than simple bilinear filtering. This requires us to use a custom shader, so create a new Bloom shader.

### Box Sampling

We're going to adjust our shader so it uses a different sampling method that bilinear filtering. Because sampling depends on the pixel size, add the magic float4 `_MainTex_TexelSize` variable to the `CGINCLUDE` block. Keep in mind that this corresponds to the texel size of the source texture, not the destination.

Instead of relying on a bilinear filter only, we'll use a simple box filter kernel instead. It takes four samples instead of one, diagonally positioned so we get the averages of four adjacent 2*2 pixels blocks. Sum these samples and divide by four, so we end up with the average of a 4*4 pixel block, doubling our kernel size.

### Different Passes

The result is much smoother and has much higher quality, but it is also much blurrier. This is mostly due to upsampling with the new 4*4 box filter. As we're using the source's texel size to position the sample points, we end up covering a large area, with an unfocused regular weight distribution. 

We can tune our box filter by adjust the UV delta that we use to select the sample points. To make this possible, turn the delta into a parameter, instead of always using 1.

## Creating Bloom

Blurring the original image is the first step of creating a bloom effect. The second step is to combine the blurred image with the original, brightening it. However, we won't just use the final blurred result, as that would produce a rather uniform smudging. Instead, lower amounts of blurring should contribute more to the result that higher amounts of blurring. We can do this by accumulating the intermediate results, adding to the old data as we upsample. 

### Additive Blending

Adding to what we already have at some intermediate level can be done by using additive blending, instead of replacing the texture's contents. All we have to do is set the blend mode of the upsampling pass to one one.
