import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { debounceTime, distinctUntilChanged, fromEvent, tap } from 'rxjs';
import { EvaluateResponseModel } from 'src/app/models/EvaluateResponseModel';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-expression',
  templateUrl: './expression.component.html',
  styleUrls: ['./expression.component.scss']
})
export class ExpressionComponent implements AfterViewInit {

  @ViewChild('expression') input: ElementRef | undefined;

  evaluationResult: EvaluateResponseModel | undefined = undefined;
  errorDetails: string | undefined = undefined;
  svgUrl: string | undefined = undefined;

  constructor(private api: ApiService) { }

  ngAfterViewInit(): void {    
    fromEvent(this.input?.nativeElement, 'keyup')
      .pipe(
        debounceTime(150),
        distinctUntilChanged(),
        tap(_ => {
          const expr = this.input?.nativeElement.value;
          this.evaluate(expr);           
        })
      ).subscribe();
  }

  evaluate(expr: string) {
    this.api.evaluate(expr).subscribe(
      res => {
        this.evaluationResult = res;
        this.errorDetails = undefined;
        this.svgUrl = `/api/svg?expression=${encodeURIComponent(expr)}`;
      },
      err => {
        this.evaluationResult = { value: '', log: '' };
        this.errorDetails = err.error.title;
      } 
    );
  }
}
