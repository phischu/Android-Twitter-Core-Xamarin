# Android-Twitter-Core-Xamarin
Xamarin bindings for the twitter android sdk
## Xamarin bindings for twitter android sdk core

In this document we document how we got very rough but working [Xamarin bindings](http://developer.xamarin.com/guides/android/advanced_topics/java_integration_overview/binding_a_java_library_(.jar)/) for the [twitter android sdk core](https://dev.twitter.com/twitter-kit/android/twittercore). Disclaimer: this is probably not how you do it.

### Installation with visual studio

Do not use these bindings directly. They are really quick and really dirty. If you still want to:

- Clone this repository into an arbitrary folder.
- Select your solution in visual studio.
- File -> Add -> Existing Project -> Choose this .csproj file.
- Add a reference in your android project.

### How we got here

The main reference is [this guide](http://developer.xamarin.com/guides/android/advanced_topics/java_integration_overview/binding_a_java_library_(.jar)/). 

We created a new project from the "Xamarin Bindings library" project.

#### Getting the jar

We located the relevant java archive at a [private twitter maven repository](https://twittersdk.artifactoryonline.com/twittersdk/public/com/twitter/sdk/android/twitter-core/1.4.0/). The java archive is hidden in [another archive](http://tools.android.com/tech-docs/new-build-system/aar-format) (ending in `.aar` but actually just a `.zip` archive). In this archive there is a `classes.jar` file that we extracted and renamed to `twitter-core-1.4.0.jar`.

#### Getting the depedencies

In the [maven repository for twitter core](https://twittersdk.artifactoryonline.com/twittersdk/public/com/twitter/sdk/android/twitter-core/1.4.0/) we looked at the `pom` file and manually and transitively chased dependencies on other artifacts. This got us `gson-2.2.4.jar`, `fabric-1.3.4.jar` and `retrofit-1.6.1.jar`. We noticed that for [some maven artifacts](http://mvnrepository.com/artifact/com.squareup.retrofit/retrofit/1.6.1) some dependencies are optional and omitted all of those.

#### Adding the jars to the project

We added all four jars to the `Jars` folder in our project. We chose build action `EmbeddedJar` for `twitter-core-1.4.0.jar` and `fabric-1.3.4.jar` and build action `EmbeddedReferenceJar` for `gson-2.2.4.jar` and `retrofit-1.6.1.jar`. Contrary to what the accompanying `AboutJars.txt` says we did not choose `InputJar` and `ReferenceJar`. We did not [know beforehand](http://stackoverflow.com/questions/26629838/twitter-fabric-login-for-android) that we would have to directly use `fabric`. Only later we set its build action from `EmbeddedReferenceJar` to `EmbeddedJar` to generate bindings for it as well.

#### Fixing errors

The errors we had to fix fall into four categories:

1. Methods with the same name as the class. Easy to fix by specifying a renaming in `Metadata.xml` like so:
    `<attr path="/api/package[@name='com.twitter.sdk.android.core.models']/class[@name='Coordinates']/field[@name='coordinates']" name="name">Coordinats</attr>`
2. Some classes were private but should be public. Easy to fix by overriding the visibility in `Metadata.xml`:
    `<attr path="/api/package[@name='com.twitter.sdk.android.core.models']/class[@name='Entity']" name="visibility">public</attr>`
3. An interface was missing. We created an empty interface in `Additions/IAppSpiCall.cs`.
4. An overriding method returned `ICollection<ITask>` while the overriden method returned `ICollection`. We added a method that does an explicit cast in `Additions/PriorityTask.cs`.

It was helpful that [twitter-kit-android](https://github.com/twitter/twitter-kit-android) is on github. Fixing build errors for `fabric` was more difficult and involved inspecting its jar because we did not have the source code. The `Metadata.xml` language has a [reference](http://developer.xamarin.com/guides/android/advanced_topics/java_integration_overview/binding_a_java_library_(.jar)/api_metadata_reference/).

#### Adding resources

Twitter core for android needs [resources](https://github.com/twitter/twitter-kit-android/tree/master/twitter-core/src/main/res). They are present in the jar in a `res` folder. When building an android `apk` the build tool usually [generates and compiles](http://spin.atomicobject.com/2011/08/22/building-android-application-bundles-apks-by-hand/) an `R.java` file and also puts it into the jar. Unfourtunately `twitter-core-1.4.0.jar` was missing `R.class` and several related classes like `R$layout.class`. It did feature an `R.txt` with the same information as `R.java` would have, probably for [other build tools](http://buckbuild.com/rule/android_resource.html).

We ussed `aapt` to generate `R.java` by hand. Exact command lost but something like:

    aapt p -m -J gen/ -M ./AndroidManifest.xml -S res/ -I android.jar

The result is generated in the `gen/` folder.

    cd gen

We used `javac` to compile it to `.class` files:

    javac com/twitter/sdk/android/core/R.java

We used the `jar` tool to add those to `twitter-core-1.4.0.jar`:

    jar uf twitter-core-1.4.0.jar com/twitter/sdk/android/core/*

We replaced our original `twitter-core-1.4.0.jar` with this one.

We had to include the resource files themselves. We zipped the `res` folder (called `res.zip`), added it to the project and set its build action to `LibraryProjectZip`. Luckily this worked.
