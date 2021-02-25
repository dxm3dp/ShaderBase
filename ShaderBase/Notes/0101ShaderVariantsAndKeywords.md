# Shader variants and keywords

[Shader Reference](https://docs.unity3d.com/Manual/SL-MultipleProgramVariants.html)

You can write shader snippets that share common code, but have different functionality when a given keyword is enabled or disabled. When Unity compiles these shader snippets, it creates separate shader programs for the different combinations of enabled and disabled keywords. These individual shader programs are called **shader variants**.

Shader variants can be useful for project workflow reasons; you can assign the same shader to different Materials, but configure the keyword differently for each. This means that you write and maintain your shader code in a single place, and have fewer shader assets in your project. You can also use shader variants to change shader behaviour at runtime, by enabling or disabling keywords.

Shaders with a large number of variants are called "mega shaders" or "uber shaders".

## Using shader variants and keywords

To create shader variants, you add one of the following directives to your shader snippet:

- `#pragma multi_compile`
- `#pragma multi_compile_local`
- `#pragma shader_feature`
- `#pragma shader_feature_local`

You can use these directives in regular shaders and compute shaders.

Unity then compiles the same shader code multiple times with different preprocessor directives.

## Enabling and disabling shader keywords

To enable and disable shader keywords, use the following APIs:

- `Shader.EnableKeyword`: enable a global keyword
- `Shader.DisableKeyword`: disable a global keyword
- `Meterial.EnableKeyword`: enable a local keyword for a regular shader
- `Material.DisableKeyword`: disable a local keyword for a regular shader

When you enable or disable a keyword, Unity uses the appropriate variant.

## How multi_compile works

Example directive:

`#pragma multi_compile FANCY_STUFF_OFF FANCY_STUFF_ON`

This example directive produces two shader variants: one with `FANCY_STUFF_OFF` defined, and another with `FANCY_STUFF_ON`. At run time, Unity activates one of them based on the Material or global shader keywords. If neither of these two keywords are enabled, then Unity uses the first one. 

You can add more than two keywords on a multi_compile line. For example:

`#pragma multi_compile SIMPLE_SHADING BETTER_SHADING GOOD_SHADING BEST_SHADING`

This example directive produces four shader variants.

To produce a shader variant with no preprocessor macro defined, add a name that is just underscores(`__`). This is a common technique to avoid using up two keywords, because there is a limit on how many you can use in a project. For example:

`#pragma multi_compile __ FOO_ON`

This directive produces two shader variants: one with nothing defined(`__`), and one with `FOO_ON` defined.

## Combining several multi_compile lines

If you provide `multi_compile` lines, Unity complies the resulting shader for all possible combinations of the lines. For example:

`#pragma multi_compile A B C`

`#pragma multi_compile D E`

This produces three variants for the first line, and two for the second line. In total, it produces total six shader variants.

Think of each `multi_compile` line as controlling a single shader "feature". 
