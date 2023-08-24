import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
    id("java")
    id("org.jetbrains.kotlin.jvm") version "1.6.21"
    id("org.jetbrains.intellij") version "1.8.0"
    id("io.gitlab.arturbosch.detekt") version "1.21.0"
    id("org.jlleitschuh.gradle.ktlint") version "10.3.0"
}

repositories {
    mavenCentral()
    jcenter()
}

val jvmVersion = "11"
val kotlinVersion = "1.6" // should match org.jetbrains.kotlin.jvm (major.minor)

intellij {
    pluginName.set("my cool plugin")
    version.set("2022.2")
    type.set("RD")
    downloadSources.set(true)
    updateSinceUntilBuild.set(false)
}

tasks {

    clean {
    }

    withType<JavaCompile> {
        sourceCompatibility = jvmVersion
        targetCompatibility = jvmVersion
    }
    withType<KotlinCompile> {
        kotlinOptions {
            jvmTarget = jvmVersion
            languageVersion = kotlinVersion
            apiVersion = kotlinVersion
            freeCompilerArgs = listOf("-Xjvm-default=compatibility")
        }
    }
    buildSearchableOptions {
        enabled = false
    }
    test {
        useJUnitPlatform()
    }
}
