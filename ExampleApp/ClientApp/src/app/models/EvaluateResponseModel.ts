import { VariantKind } from "./VariantKind";

export interface EvaluateResponseModel {
    kind: VariantKind;
    value: string;
}