/*
 * Licensed to the .NET Foundation under one or more agreements.
 * The .NET Foundation licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
 */

package org.apache.spark.api.dotnet

import org.apache.spark.ml._
import scala.collection.JavaConverters._

/** DotnetHelper object that hosts some helper functions
  * help data type conversions between dotnet and scala
  */
object DotnetHelper {

  /** A helper function to let pipeline accept java.util.ArrayList
    * format stages in scala code
    * @param pipeline - The pipeline to be set stages
    * @param value - A java.util.ArrayList of PipelineStages to be set as stages
    * @return The pipeline
    */
  def setPipelineStages(pipeline: Pipeline, value: java.util.ArrayList[_ <: PipelineStage]): Pipeline =
    pipeline.setStages(value.asScala.toArray)

  /** A helper function to convert scala Map to java.util.Map
    * @param value - scala Map
    * @return java.util.Map
    */
  def convertToJavaMap(value: Map[_, _]): java.util.Map[_, _] = value.asJava

  /** Convert java data type to corresponding scala type
    * @param value - java.lang.Object
    * @return Any
    */
  def mapScalaToJava(value: java.lang.Object): Any = {
    value match {
      case i: java.lang.Integer => i.toInt
      case d: java.lang.Double => d.toDouble
      case f: java.lang.Float => f.toFloat
      case b: java.lang.Boolean => b.booleanValue()
      case l: java.lang.Long => l.toLong
      case s: java.lang.Short => s.toShort
      case by: java.lang.Byte => by.toByte
      case c: java.lang.Character => c.toChar
      case _ => value
    }
  }
}
