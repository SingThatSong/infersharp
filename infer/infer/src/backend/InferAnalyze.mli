(*
 * Copyright (c) 2009-2013, Monoidics ltd.
 * Copyright (c) 2013-present, Facebook, Inc.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 *)

open! IStd

(** Main module for the analysis after the capture phase *)

val analyze_json: string -> string -> unit

val main : changed_files:SourceFile.Set.t option -> unit
(** Given a name of the Makefile to use for multicore analysis, analyze the captured code *)
